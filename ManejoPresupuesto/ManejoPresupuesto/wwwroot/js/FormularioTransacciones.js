function inicializarFormularioTransacciones(urlObtenerCategorias) {
    $("#TipoOperacionId").change(async function () {
        const ValorSeleccionado = $(this).val();

        const respuesta = await fetch(urlObtenerCategorias, {
            method: 'POST',
            body: ValorSeleccionado,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const catego = await respuesta.json();
        console.log(catego);
        const opciones = catego.map(categoria => `<option value=${categoria.value}>${categoria.text}</option>`)
        $("#CategoriaId").html(opciones);

    })
}