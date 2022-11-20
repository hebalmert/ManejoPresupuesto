using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategoria _repositorioCategoria;
        private readonly IServiciosUsuarios _serviciosUsuarios;

        public CategoriasController(IRepositorioCategoria repositorioCategoria,
            IServiciosUsuarios serviciosUsuarios)
        {
            _repositorioCategoria = repositorioCategoria;
            _serviciosUsuarios = serviciosUsuarios;
        }


        //Metodo con LinQ de GroupBy
        public async Task<IActionResult> Index()
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var categorias = await _repositorioCategoria.Obtener(usuarioId);

            return View(categorias);
        }


            [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            categoria.UsuarioId = usuarioId;
            await _repositorioCategoria.Crear(categoria);



            return  RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var categoria = await _repositorioCategoria.ObtnerPorId(id, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoriaEdit)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaEdit);
            }

            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var categoria = await _repositorioCategoria.ObtnerPorId(categoriaEdit.Id, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
           categoriaEdit.UsuarioId= usuarioId;

            await _repositorioCategoria.Actualizar(categoriaEdit);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var categoria = await _repositorioCategoria.ObtnerPorId(id, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuario();
            var categoria = await _repositorioCategoria.ObtnerPorId(id, usuarioId);
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _repositorioCategoria.Borrar(id);
            return RedirectToAction("Index");

        }
    }
}
