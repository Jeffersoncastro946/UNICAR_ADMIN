window.initWizardVehiculo = function (container) {
    // Busca dentro del contenedor (el modal) los elementos del wizard
    const $container = container ? $(container) : $(document);

    // Paso 1
    const btnPaso1 = $container.find('#paso1 a[data-bs-toggle="list"]')[0];
    if (btnPaso1) {
        btnPaso1.addEventListener("click", function (e) {
            e.preventDefault();
            const navPaso2 = $container.find('.list-group a[href="#paso2"]')[0];
            if (navPaso2) {
                bootstrap.Tab.getOrCreateInstance(navPaso2).show();
            }
        });
    }

    // Paso 2
    const btnPaso2 = $container.find('#paso2 a[data-bs-toggle="list"]')[0];
    if (btnPaso2) {
        btnPaso2.addEventListener("click", function (e) {
            e.preventDefault();
            const navPaso3 = $container.find('.list-group a[href="#paso3"]')[0];
            if (navPaso3) {
                bootstrap.Tab.getOrCreateInstance(navPaso3).show();
            }
        });
    }

    // Llenar dropdowns y datos del proveedor
    cargarProveedores();
    // cargarDatosProveedor();

    // Configurar vistas previas de imágenes
    setupImagePreview('FotoFrontalFile', 'preview-FotoFrontal');
    setupImagePreview('FotoTraseraFile', 'preview-FotoTrasera');
    setupImagePreview('FotoLateralIzquierdaFile', 'preview-FotoLateralIzquierda');
    setupImagePreview('FotoLateralDerechaFile', 'preview-FotoLateralDerecha');
    setupImagePreview('FotoInterior1File', 'preview-FotoInterior1');
    setupImagePreview('FotoInterior2File', 'preview-FotoInterior2');
    setupImagePreview('FotoMotorFile', 'preview-FotoMotor');
    setupImagePreview('FotoExtra1File', 'preview-FotoExtra1');
    setupImagePreview('FotoExtra2File', 'preview-FotoExtra2');
};