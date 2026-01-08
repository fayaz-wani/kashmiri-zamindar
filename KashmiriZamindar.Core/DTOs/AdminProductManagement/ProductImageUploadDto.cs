using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KashmiriZamindar.Core.Dtos
{
    public class ProductImageUploadDto
    {
        public string Base64Image { get; set; }  // Image as base64 string
        public string FileName { get; set; }
        public bool IsPrimary { get; set; }
    }


}
