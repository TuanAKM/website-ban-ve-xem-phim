using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.Models
{
    public class PhongChieu
    {
        [Key]
        [Required(ErrorMessage = "Vui lòng nhập mã phòng")]
        [RegularExpression(@"^[a-zA-Z0-9\-]+$", ErrorMessage = "Mã phòng chỉ được chứa chữ cái, số và dấu gạch ngang (không chứa dấu cách hay ký tự đặc biệt)")]
        public string MaPhong { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng nhập tên phòng")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ]+$", ErrorMessage = "Tên phòng không được chứa ký tự đặc biệt (!@#$...)")]
        public string TenPhong { get; set; } = null!;
        [Required(ErrorMessage = "Vui lòng chọn Loại Phòng")]
        public string LoaiPhong { get; set; } = null!;
        public int TongSoGhe { get; set; }
        
        public ICollection<Ghe>? Ghes { get; set; }
        public ICollection<SuatChieu>? SuatChieus { get; set; }
    }
}
