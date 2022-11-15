using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.ObjectModelRemoting;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IServiciosUsuarios _serviciosUsuarios;
        private readonly IRepositorioCuentas _repositorioCuentas;
        private readonly IMapper _mapper;

        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServiciosUsuarios serviciosUsuarios,
            IRepositorioCuentas repositorioCuentas,
            IMapper mapper)
        {
            _repositorioTiposCuentas = repositorioTiposCuentas;
            _serviciosUsuarios = serviciosUsuarios;
            _repositorioCuentas = repositorioCuentas;
            _mapper = mapper;
        }


        //Metodo con LinQ de GroupBy
        public async Task<IActionResult> Index()
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var cuentasConTipoCuenta = await _repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndeceCuentasViewModel
                { 
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            return View(modelo);  
        }



        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            CuentaCreacionViewModel modelo = new();
            //llamamos el Enumerable<SelectListItem> para llenarlo
           modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId); 

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            var tipocuenta = await _repositorioTiposCuentas.ObtenerPorId(cuenta.TiposCuentasId, usuarioid);
            if (tipocuenta is null) 
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioid);
                return View(cuenta);
            }

            await _repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id) 
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            var cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioid);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //CuentaCreacionViewModel modelo = new() 
            //{ 
            //    Id= id,
            //    Nombre= cuenta.Nombre,
            //    TiposCuentasId= cuenta.TiposCuentasId,
            //    Descripcion=cuenta.Descripcion,
            //    Balance=cuenta.Balance
            //};

            //Todo lo anterior se recumen a esto, gracias al AutoMapper
            var modelo = _mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioid);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioid = _serviciosUsuarios.ObtenerUsuario();
            var cuenta = await _repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioid);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var tipocuenta = await _repositorioTiposCuentas.ObtenerPorId(cuentaEditar.TiposCuentasId, usuarioid);
            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //necesitamos un nuevo metodo desde el Rpositorio de Cuentas
            await _repositorioCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposcuentas = await _repositorioTiposCuentas.Obtener(usuarioId);
            return tiposcuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
