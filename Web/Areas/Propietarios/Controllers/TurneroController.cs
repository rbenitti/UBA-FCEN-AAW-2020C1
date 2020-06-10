﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.UseCases.Cliente;
using Aplicacion.UseCases.Propietario;
using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Propietarios.Models;

namespace Web.Areas.Propietarios.Controllers
{

    [Area("Propietarios")]
    //[Authorize]
    [AllowAnonymous]
    public class TurneroController : Controller
    {

        private readonly ICurrentUserService _userService;

        public TurneroController(ICurrentUserService us)
        {
            _userService = us;
        }


        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(CrearTurneroVM turnero, [FromServices] CrearTurneroUC uc)
        {
            var req = new CrearTurneroRequest
            {
                IdPropietario = _userService.UserId,
                Ciudad = turnero.Ciudad,
                Calle = turnero.Calle,
                Numero = turnero.Numero,
                Concepto = turnero.Concepto,
                Ubicacion = new LatLon(double.Parse(turnero.Latitud), double.Parse(turnero.Longitud)),
                CantidadMaxima = turnero.CantidadMaxima
            };

            uc.Procesar(req);

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Detalle(int idTurnero, [FromServices] DetalleTurneroUC uc)
        {
            var req = new DetalleTurneroRequest { IdTurnero = idTurnero };
            var response = uc.Procesar(req);

            var infoTurnero = new InformacionTurneroVM
            {
                IdTurnero = response.IdTurnero,
                CantidadEnEspera = response.CantidadEnEspera,
                Concepto = response.Concepto,
                Ciudad = response.Ciudad,
                Direccion = $"{response.Calle} {response.Numero}",
                Qr = response.QrTurnero,
                CantidadMaxima = response.CantidadMaxima,
                Latitud = response.Latitud,
                Longitud = response.Longitud
            };

            var detalleTurneroVM = new DetalleTurneroVM()
            {
                InfoTurnero = infoTurnero,
                NumeroTurnoEnLlamada = response.NumeroTurnoEnLlamada
            };

            return View(detalleTurneroVM);
        }


        public IActionResult CancelarTurno(int idTurno, int idTurnero, [FromServices] CancelarTurnoUC uc)
        {
            //REVISAR EL REQUEST SOBRE CLIENTE UC
            var req = new CancelarTurnoRequest
            {
                IdTurnero = idTurnero,
                IdTurno = idTurno
            };

            uc.Procesar(req);

            return RedirectToAction(nameof(Detalle), new { idTurnero });
        }

        [HttpPost]
        public IActionResult SaltarTurno(int idTurnero, [FromServices] SaltarTurnoUC uc)
        {
            var req = new SaltarTurnoRequest
            {
                IdTurnero = idTurnero
            };

            uc.Procesar(req);

            return RedirectToAction(nameof(Detalle), new { idTurnero });
        }

        public IActionResult DemorarTurno(int idTurnero, [FromServices] DemorarTurnoUC uc)
        {
            var req = new DemorarTurnoRequest { IdTurnero = idTurnero };
            uc.Procesar(req);

            return RedirectToAction(nameof(Detalle), new { idTurnero });
        }


        [HttpGet]
        public IActionResult Editar(int idTurnero, [FromServices] SaltarTurnoUC uc)
        {
            var req = new SaltarTurnoRequest { IdTurnero = idTurnero };
            var detalleTurnero = uc.Procesar(req);
            var turnero = new EditarTurneroVM(
                idTurnero,
                detalleTurnero.Concepto,
                detalleTurnero.Ciudad,
                detalleTurnero.Calle,
                detalleTurnero.Numero,
                detalleTurnero.Latitud,
                detalleTurnero.Longitud,
                detalleTurnero.CantidadMaxima);

            return View(turnero);
        }

        [HttpPost]
        public IActionResult Editar(EditarTurneroVM turnero, [FromServices] EditarTurneroUC uc)
        {
            var req = new EditarTurneroRequest
            {
                IdPropietario = _userService.UserId,
                IdTurnero = turnero.IdTurnero,
                Ciudad = turnero.Ciudad,
                Calle = turnero.Calle,
                Numero = turnero.Numero,
                Concepto = turnero.Concepto,
                Ubicacion = new LatLon(turnero.Latitud, turnero.Longitud),
                CantidadMaxima = turnero.CantidadMaxima
            };

            uc.Procesar(req);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public string AtenderTurno(string qrData)
        {

            return qrData;
        }

    }
}
