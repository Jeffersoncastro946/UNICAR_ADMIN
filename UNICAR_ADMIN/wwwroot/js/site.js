document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function () {
            const sb = document.getElementById('sidebarMenu');
            if (sb) {
                sb.classList.toggle('collapsed');
            }
            const icon = this.querySelector('i');
            if (icon) {
                icon.classList.toggle('bi-chevron-bar-left');
                icon.classList.toggle('bi-chevron-bar-right');
            }
        });
    }
});
//function abrirModal(tamano, titulo, url) {
//    const modal = $('#modalGlobal');
//    const dialog = modal.find('.modal-dialog');

//    // Quita cualquier tamaño previo
//    dialog.removeClass('modal-sm modal-md modal-lg');

//    // Aplica el tamaño
//    if (tamano === 'sm') dialog.addClass('modal-sm');
//    else if (tamano === 'md') dialog.addClass('modal-md');
//    else if (tamano === 'lg') dialog.addClass('modal-lg');

//    // Asigna el título
//    modal.find('.modal-title').text(titulo);

//    // Carga el contenido desde la URL
//    $.get(url, function (html) {
//        const body = modal.find('.modal-body');
//        body.html(html);
//        modal.modal('show');
//    });

//}

function abrirModal(tamano, titulo, url) {
    const modal = $('#modalGlobal');
    const dialog = modal.find('.modal-dialog');

    // Quita cualquier tamaño previo
    dialog.removeClass('modal-sm modal-md modal-lg');

    // Aplica el tamaño
    if (tamano === 'sm') dialog.addClass('modal-sm');
    else if (tamano === 'md') dialog.addClass('modal-md');
    else if (tamano === 'lg') dialog.addClass('modal-lg');

    // Asigna el título
    modal.find('.modal-title').text(titulo);

    // Carga el contenido desde la URL
    $.get(url, function (html) {
        const body = modal.find('.modal-body');
        body.html(html);

        // 👇🏻 PARSEA VALIDACIÓN UNOBTRUSIVE AQUÍ 👇🏻
        $.validator.unobtrusive.parse(body);

        modal.modal('show');
    });
}




/**
 * Muestra una alerta de Bootstrap dentro de #alertContainer.
 * @param {string} type    - 'success','danger','warning','info',...
 * @param {string} message - El texto a mostrar.
 * @param {number} [timeout=5000] - Tiempo en ms para auto-cerrar (opcional).
 */
function showAlert(type, message, timeout = 5000) {
    // Generar un ID único
    const alertId = 'alert-' + Date.now();

    // HTML con template literal
    const html = `
    <div id="${alertId}"
         class="alert alert-${type} alert-dismissible fade show"
         role="alert">
      ${message}
      <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>`;

    // Insertar en el contenedor
    $('#alertContainer').append(html);

    // Auto–cierre
    if (timeout > 0) {
        setTimeout(() => {
            $('#' + alertId).alert('close');
        }, timeout);
    }
}
