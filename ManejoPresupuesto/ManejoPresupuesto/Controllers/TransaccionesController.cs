using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host.Mef;

namespace ManejoPresupuesto.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IServiciosUsuarios _serviciosUsuarios;
        private readonly IRepositorioCuentas _repositorioCuentas;
        private readonly IRepositorioCategoria _repositorioCategoria;
        private readonly IRepositorioTransacciones _repositorioTransacciones;

        public TransaccionesController(IServiciosUsuarios serviciosUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategoria repositorioCategoria,
            IRepositorioTransacciones repositorioTransacciones)
        {
            _serviciosUsuarios = serviciosUsuarios;
            _repositorioCuentas = repositorioCuentas;
            _repositorioCategoria = repositorioCategoria;
            _repositorioTransacciones = repositorioTransacciones;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenetCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionViewModel modelo)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenetCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await _repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await _repositorioCategoria.ObtnerPorId(modelo.CategoriaId, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;

            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                modelo.Monto *= -1;
            }

            await _repositorioTransacciones.Crear(modelo);
            return RedirectToAction("Index");
        }

        public async Task<IEnumerable<SelectListItem>> ObtenetCuentas(int usuarioId)
        {
            var cuentas = await _repositorioCuentas.Buscar(usuarioId);
            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacion tipoOperacion)
        {
            var categorias = await _repositorioCategoria.Obtener(usuarioId, tipoOperacion);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacion tipoOperacion)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }
    }
}
