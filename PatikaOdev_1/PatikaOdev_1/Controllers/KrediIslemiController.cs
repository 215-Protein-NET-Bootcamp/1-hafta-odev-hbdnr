using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PatikaOdev_1.Models;
using PatikaOdev_1.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PatikaOdev_1.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class KrediIslemiController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Options _options;

        public KrediIslemiController(IConfiguration configuration)
        {
            _configuration = configuration;
            _options = configuration.GetSection("options").Get<Options>();
        }

        public double toplamFaizMiktari = 0;
        // GET api/<KrediIslemi>/5
        [HttpGet]
        [Route("Faiz")]
        public IActionResult GetFaiz(double vadeTutari, double istenilenMiktar)
        {
            var result = ValidationResult(vadeTutari, istenilenMiktar);

            if (result != null)
                return BadRequest(result);

            List<Faiz> list = new();

            //Anuite formülü
            //float faizOrani = float.Parse(_configuration["FaizOrani"]);
            double faizOrani = _options.FaizOrani;
            double donemSayisi = vadeTutari;
            double AlinanKrediTutari = istenilenMiktar;
            double esitTaksitTutari;
            double toplamTaksitTutari;



            double formul1 = ((Math.Pow((1 + faizOrani), donemSayisi)) - 1) / ((Math.Pow((1 + faizOrani), donemSayisi)) * faizOrani);

            esitTaksitTutari = AlinanKrediTutari / formul1;

            toplamTaksitTutari = esitTaksitTutari * donemSayisi;

            for (int i = 1; i < donemSayisi + 1; i++)
            {
                double dusulecekMiktar = 0;
                dusulecekMiktar = AlinanKrediTutari * faizOrani;
                toplamFaizMiktari += dusulecekMiktar;
                double azalacakMiktar = esitTaksitTutari - dusulecekMiktar;
                AlinanKrediTutari -= azalacakMiktar;
            }

            list.Add(new Faiz { VadeTutari = toplamTaksitTutari, IstenilenMiktar = toplamFaizMiktari });
            return Ok(list);
        }

        [HttpGet]
        [Route("OdemePlani")]
        public IActionResult GetOdemePlani(double vadeTutari, double istenilenMiktar)
        {
            var result = ValidationResult(vadeTutari, istenilenMiktar);

            if (result != null)
                return BadRequest(result);

            List<OdemePlani> list = new();

            //Anuite formülü
            double faizOrani = _options.FaizOrani;
            double donemSayisi = vadeTutari;
            double AlinanKrediTutari = istenilenMiktar;
            double esitTaksitTutari;
            double toplamTaksitTutari;

            //list.Add(new OdemePlani { ay = 000, taksitTutari = faizOrani, faiz = 000, anaPara = 0000, bakiye = AlinanKrediTutari });

            double formul1 = ((Math.Pow((1 + faizOrani), donemSayisi)) - 1) / ((Math.Pow((1 + faizOrani), donemSayisi)) * faizOrani);

            esitTaksitTutari = AlinanKrediTutari / formul1;

            toplamTaksitTutari = esitTaksitTutari * donemSayisi;

            for (int i = 1; i < donemSayisi + 1; i++)
            {
                double dusulecekMiktar = 0;
                dusulecekMiktar = AlinanKrediTutari * faizOrani;
                toplamFaizMiktari += dusulecekMiktar;
                double azalacakMiktar = esitTaksitTutari - dusulecekMiktar;
                AlinanKrediTutari -= azalacakMiktar;
                if (i == donemSayisi)
                {
                    AlinanKrediTutari = 0;
                }
                list.Add(new OdemePlani { Ay = i, TaksitTutari = esitTaksitTutari, Faiz = dusulecekMiktar, AnaPara = azalacakMiktar, Bakiye = AlinanKrediTutari });

            }

            return Ok(list);
        }

        private IValidationResult ValidationCheckNullistenilenMiktar(double? IstenilenMiktar)
        {
            if (IstenilenMiktar == null || IstenilenMiktar == 0)
                return new ErrorValidationResult(_options.IstenenMiktarNull);
            return new SuccessValidationResult();
        }

        private IValidationResult ValidationCheckNegativeistenilenMiktar(double IstenilenMiktar)
        {
            if (IstenilenMiktar < 0)
                return new ErrorValidationResult(_options.IstenenMiktarNegatif);
            return new SuccessValidationResult();
        }

        private IValidationResult ValidationCheckNegativevadeTutari(double VadeTutari)
        {
            if (VadeTutari < 0)
                return new ErrorValidationResult(_options.VadeTutariNegatif);
            return new SuccessValidationResult();
        }

        private IValidationResult ValidationCheckNullvadeTutari(double? VadeTutari)
        {
            if (VadeTutari == null || VadeTutari == 0)
                return new ErrorValidationResult(_options.VadeTutariNull);
            return new SuccessValidationResult();
        }

        private IValidationResult ValidationResult(double VadeTutari, double IstenilenMiktar)
        {
            return ValidationHelper.Run(
                ValidationCheckNullistenilenMiktar(IstenilenMiktar),
                ValidationCheckNegativeistenilenMiktar(IstenilenMiktar),
                ValidationCheckNegativevadeTutari(VadeTutari),
                ValidationCheckNullvadeTutari(VadeTutari)
                );
        }

    }
}
