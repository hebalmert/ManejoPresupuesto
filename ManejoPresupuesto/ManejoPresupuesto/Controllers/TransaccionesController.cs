using AutoMapper;
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
        private readonly IMapper _mapper;

        public TransaccionesController(IServiciosUsuarios serviciosUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategoria repositorioCategoria,
            IRepositorioTransacciones repositorioTransacciones,
            IMapper mapper)
        {
            _serviciosUsuarios = serviciosUsuarios;
            _repositorioCuentas = repositorioCuentas;
            _repositorioCategoria = repositorioCategoria;
            _repositorioTransacciones = repositorioTransacciones;
            _mapper = mapper;
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

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();

            var transacciones = await _repositorioTransacciones.ObtenerPorId(id, usuarioid);
            if (transacciones is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var modelo = _mapper.Map<TransaccionActualizacionViewModel>(transacciones);

            modelo.MontoAnterior = modelo.Monto;
            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            { 
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorId = transacciones.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioid, transacciones.TipoOperacionId);
            modelo.Cuentas = await ObtenetCuentas(usuarioid);


            return View(modelo);
        }


        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionViewModel modelo)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenetCuentas(usuarioid);
                modelo.Categorias = await ObtenerCategorias(usuarioid, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await _repositorioCuentas.ObtenerPorId(modelo.CuentaId, usuarioid);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await _repositorioCategoria.ObtnerPorId(modelo.CategoriaId, usuarioid);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = _mapper.Map<Transaccion>(modelo);
            if (modelo.TipoOperacionId == TipoOperacion.Gasto)
            {
                transaccion.Monto *= -1;
            }

            await _repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            var transaccion = await _repositorioTransacciones.ObtenerPorId(id, usuarioid);
            if (transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await _repositorioTransacciones.Borrar(id);
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
