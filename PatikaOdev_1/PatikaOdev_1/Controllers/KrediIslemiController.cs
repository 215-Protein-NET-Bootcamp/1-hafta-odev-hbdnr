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
        //appsettings dosyasında tanımlanan özelliklere erişmek iin eklendi.
        private readonly Options _options;

        public KrediIslemiController(IConfiguration configuration)
        {
            _configuration = configuration;
            _options = configuration.GetSection("options").Get<Options>();
        }

        public double toplamFaizMiktari = 0;
        // GET api/<KrediIslemi>/5
        //Ödenecek toplam faiz ve tutarın bilgisini geri dönen metod.
        [HttpGet]
        [Route("Faiz")]
        public IActionResult GetFaiz(double vadeTutari, double istenilenMiktar)
        {
            //validation kontrollerinin uygulanması
            var result = ValidationResult(vadeTutari, istenilenMiktar);
            //dönüş null değilse yani hata varsa bağlı olunan hatayı dönecek.
            if (result != null)
                return BadRequest(result);

            List<Faiz> list = new();

            //faiz oranı (0.01)
            double faizOrani = _options.FaizOrani;
            //vade (ay) sayısı (örnek: 9)
            double donemSayisi = vadeTutari;
            //istenen toplam para miktarı (örnek: 50000)
            double AlinanKrediTutari = istenilenMiktar;
            //ödenecek eşit taksit miktarı
            double esitTaksitTutari;
            //faiz ile beraber toplam ödenecek para
            double toplamTaksitTutari;

            //Anuite formülü
            double formul1 = ((Math.Pow((1 + faizOrani), donemSayisi)) - 1) / ((Math.Pow((1 + faizOrani), donemSayisi)) * faizOrani);
            esitTaksitTutari = AlinanKrediTutari / formul1;

            toplamTaksitTutari = esitTaksitTutari * donemSayisi;

            //faiz miktarının hesaplanması
            for (int i = 1; i < donemSayisi + 1; i++)
            {
                double dusulecekMiktar = 0;
                dusulecekMiktar = AlinanKrediTutari * faizOrani;
                toplamFaizMiktari += dusulecekMiktar;
                double azalacakMiktar = esitTaksitTutari - dusulecekMiktar;
                AlinanKrediTutari -= azalacakMiktar;
            }

            list.Add(new Faiz { VadeTutari = toplamTaksitTutari, IstenilenMiktar = toplamFaizMiktari });
            //dönüş liste şeklinde olacak ve her durumda dönecek (OK durumundan dolayı)
            return Ok(list);
        }

        [HttpGet]
        [Route("OdemePlani")]
        public IActionResult GetOdemePlani(double vadeTutari, double istenilenMiktar)
        {
            //validation kontrollerinin uygulanması
            var result = ValidationResult(vadeTutari, istenilenMiktar);
            //dönüş null değilse yani hata varsa bağlı olunan hatayı dönecek.
            if (result != null)
                return BadRequest(result);

            List<OdemePlani> list = new();

            //faiz oranı (0.01)
            double faizOrani = _options.FaizOrani;
            //vade (ay) sayısı (örnek: 9)
            double donemSayisi = vadeTutari;
            //istenen toplam para miktarı (örnek: 50000)
            double AlinanKrediTutari = istenilenMiktar;
            //ödenecek eşit taksit miktarı
            double esitTaksitTutari;

            //double toplamTaksitTutari;

            //Anuite formülü
            double formul1 = ((Math.Pow((1 + faizOrani), donemSayisi)) - 1) / ((Math.Pow((1 + faizOrani), donemSayisi)) * faizOrani);
            esitTaksitTutari = AlinanKrediTutari / formul1;

            //toplamTaksitTutari = esitTaksitTutari * donemSayisi;

            //faiz miktarı ve ödenecek para miktarının aylara göre hesaplanması ve listeye atanması
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
            //dönüş liste şeklinde olacak ve her durumda dönecek (OK durumundan dolayı)
            return Ok(list);
        }

        //validation kontrolleri için gerekli metodlar.

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

        //validation kontrollerinin uygulanması
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
