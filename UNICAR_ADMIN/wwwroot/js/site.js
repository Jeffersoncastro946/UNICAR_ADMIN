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
        modal.find('.modal-body').html(html);
        modal.modal('show');
    });
}