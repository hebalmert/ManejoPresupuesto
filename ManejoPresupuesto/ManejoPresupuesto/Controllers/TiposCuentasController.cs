using Azure.Core;
using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System.Reflection.Metadata.Ecma335;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IServiciosUsuarios _serviciosUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServiciosUsuarios serviciosUsuarios)
        {
            _repositorioTiposCuentas = repositorioTiposCuentas;
            _serviciosUsuarios = serviciosUsuarios;
        }

        public async Task<IActionResult> Index()
        {

            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var tipocuentas = await _repositorioTiposCuentas.Obtener(usuarioId);
            return View(tipocuentas);
        }


        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.UsuarioId = _serviciosUsuarios.ObtenerUsuario();

            var yaExiste = await _repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            if (yaExiste)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre),
                    $"El nombre {tipoCuenta.Nombre} ya existe.");
                return View(tipoCuenta);
            }

            await _repositorioTiposCuentas.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var tipocuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipocuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            var tipocuentaexiste = await _repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioid);
            if (tipocuentaexiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> VerificarExiste(string nombre)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var yaExiste = await _repositorioTiposCuentas.Existe(nombre, usuarioId);
            if (yaExiste)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            var tipocuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioid);
            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipocuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuentas(int id)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            var tipocuenta = await _repositorioTiposCuentas.ObtenerPorId(id, usuarioid);
            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioTiposCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            var tiposCuentas = await _repositorioTiposCuentas.Obtener(usuarioid);
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);
            //Verificar que los ID que enviaron por el Body exiten en la base de datos
            //por eso comparamos contra tiposCuentas.
            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();
            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) => new TipoCuenta() 
            { 
                Id = valor, 
                Orden = indice + 1
            }).AsEnumerable();

            await _repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
