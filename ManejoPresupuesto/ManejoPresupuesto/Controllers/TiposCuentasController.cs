using Azure.Core;
using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
            var tipocuenta =await _repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
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
    }
}
