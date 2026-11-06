
document.addEventListener("DOMContentLoaded", async () => {
    const contenedor = document.getElementById("lista-emprendimientos");

    if (!contenedor) {
        console.error("No se encontró la sección de emprendimientos.");
        return;
    }

    // Imagen predeterminada cuando el emprendedor no ha subido una.
    const imagenPredeterminada =
        "https://images.unsplash.com/photo-1486427944299-d1955d23e34d?w=600";

    // Estilos de las tarjetas.
    const estilos = document.createElement("style");

    estilos.textContent = `
        .tarjeta {
            background: #fffdfb;
            border-radius: 20px;
            overflow: hidden;
            box-shadow: 0 6px 22px rgba(120, 90, 100, 0.12);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
            border: 1px solid #f4e5e9;
            max-width: 340px;
            width: 100%;
            margin: 12px;
        }

        .tarjeta:hover {
            transform: translateY(-7px);
            box-shadow: 0 12px 28px rgba(120, 90, 100, 0.20);
        }

        .tarjeta-imagen {
            width: 100%;
            height: 210px;
            object-fit: cover;
            display: block;
        }

        .tarjeta-contenido {
            padding: 22px;
            text-align: left;
        }

        .tarjeta-etiqueta {
            display: inline-block;
            background: #fce8ed;
            color: #a34f69;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            margin-bottom: 12px;
        }

        .tarjeta h3 {
            color: #69475a;
            font-size: 22px;
            margin: 0 0 12px;
            font-weight: 700;
        }

        .tarjeta-descripcion {
            color: #777078;
            font-size: 14px;
            line-height: 1.7;
            margin-bottom: 16px;
        }

        .tarjeta-municipio {
            color: #7b8d78;
            font-size: 13px;
            font-weight: 600;
            border-top: 1px solid #f1e9e9;
            padding-top: 14px;
            margin: 0;
        }

        .tarjeta-boton {
            display: inline-block;
            margin-top: 16px;
            padding: 10px 17px;
            background: #e9d9f2;
            color: #684c78;
            border-radius: 22px;
            text-decoration: none;
            font-size: 13px;
            font-weight: 600;
            transition: background 0.3s ease;
        }

        .tarjeta-boton:hover {
            background: #d9c1e8;
        }
    `;

    document.head.appendChild(estilos);

    try {
        const respuesta = await fetch("/api/Emprendimiento/publicos");

        if (!respuesta.ok) {
            throw new Error("Error al consultar la API.");
        }

        const emprendimientos = await respuesta.json();

        contenedor.innerHTML = "";

        if (emprendimientos.length === 0) {
            contenedor.innerHTML =
                "<p>Todavía no hay emprendimientos registrados.</p>";
            return;
        }

        emprendimientos.forEach(emprendimiento => {
            const tarjeta = document.createElement("article");
            tarjeta.className = "tarjeta";

            // Imagen propia del emprendimiento.
            const imagen = document.createElement("img");
            imagen.className = "tarjeta-imagen";

            imagen.src = emprendimiento.imagenUrl
                ? emprendimiento.imagenUrl
                : imagenPredeterminada;

            imagen.alt = "Imagen de " + emprendimiento.nombre;

            // Si la imagen guardada no carga, utiliza la predeterminada.
            imagen.onerror = () => {
                imagen.onerror = null;
                imagen.src = imagenPredeterminada;
            };

            // Contenido.
            const contenido = document.createElement("div");
            contenido.className = "tarjeta-contenido";

            const etiqueta = document.createElement("span");
            etiqueta.className = "tarjeta-etiqueta";
            etiqueta.textContent = "Hecho con talento local";

            const titulo = document.createElement("h3");
            titulo.textContent = emprendimiento.nombre;

            const descripcion = document.createElement("p");
            descripcion.className = "tarjeta-descripcion";
            descripcion.textContent =
                emprendimiento.descripcion ||
                "Conoce este emprendimiento de nuestra región.";

            const municipio = document.createElement("p");
            municipio.className = "tarjeta-municipio";
            municipio.textContent =
                "📍 " + (emprendimiento.municipio || "Oriente Antioqueño");

            const boton = document.createElement("a");
            boton.className = "tarjeta-boton";
            boton.href = "#";
            boton.textContent = "Conocer emprendimiento";

            // Organización de la tarjeta.
            contenido.appendChild(etiqueta);
            contenido.appendChild(titulo);
            contenido.appendChild(descripcion);
            contenido.appendChild(municipio);
            contenido.appendChild(boton);

            tarjeta.appendChild(imagen);
            tarjeta.appendChild(contenido);

            contenedor.appendChild(tarjeta);
        });

    } catch (error) {
        console.error("Error:", error);
        contenedor.innerHTML =
            "<p>No fue posible cargar los emprendimientos.</p>";
    }
});

