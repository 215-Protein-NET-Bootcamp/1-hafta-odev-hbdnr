using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PatikaOdev_1.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PatikaOdev_1.Controllers
{

    public class options
    {
        public double FaizOrani { get; set; }

        public string istenenMiktarNegatif { get; set; }
        public string vadeTutariNegatif { get; set; }
        public string istenenMiktarNull { get; set; }
        public string vadeTutariNull { get; set; }
    }

    public class Faiz
    {
        public double vadeTutari { get; set; }
        public double istenilenMiktar { get; set; }
    }

    public class OdemePlani
    {
        public int ay { get; set; }
        public double taksitTutari { get; set; }
        public double faiz { get; set; }
        public double anaPara { get; set; }
        public double bakiye { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class KrediIslemi : ControllerBase
    {
        readonly IConfiguration _configuration;
        readonly options _options;

        public KrediIslemi(IConfiguration configuration)
        {
            _configuration = configuration;
            _options = configuration.GetSection("options").Get<options>();
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

            list.Add(new Faiz { vadeTutari = toplamTaksitTutari, istenilenMiktar = toplamFaizMiktari });
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
                list.Add(new OdemePlani { ay = i, taksitTutari = esitTaksitTutari, faiz = dusulecekMiktar, anaPara = azalacakMiktar, bakiye = AlinanKrediTutari });

            }

            return Ok(list);
        }

        private IValidationResult ValidationCheckNullistenilenMiktar(double? istenilenMiktar)
        {
            if (istenilenMiktar == null || istenilenMiktar == 0)
                return new ErrorValidationResult(_options.istenenMiktarNull);
            return new SuccessValidationResult();
        }

        private IValidationResult ValidationCheckNegativeistenilenMiktar(double istenilenMiktar)
        {
            if (istenilenMiktar < 0)
                return new ErrorValidationResult(_options.istenenMiktarNegatif);
            return new SuccessValidationResult();
        }

        private IValidationResult ValidationCheckNegativevadeTutari(double vadeTutari)
        {
            if (vadeTutari < 0)
                return new ErrorValidationResult(_options.vadeTutariNegatif);
            return new SuccessValidationResult();
        }

        private IValidationResult ValidationCheckNullvadeTutari(double? vadeTutari)
        {
            if (vadeTutari == null || vadeTutari == 0)
                return new ErrorValidationResult(_options.vadeTutariNull);
            return new SuccessValidationResult();
        }

        private IValidationResult ValidationResult(double vadeTutari, double istenilenMiktar)
        {
            return ValidationHelper.Run(
                ValidationCheckNullistenilenMiktar(istenilenMiktar),
                ValidationCheckNegativeistenilenMiktar(istenilenMiktar),
                ValidationCheckNegativevadeTutari(vadeTutari),
                ValidationCheckNullvadeTutari(vadeTutari)
                );
        }

    }
}
