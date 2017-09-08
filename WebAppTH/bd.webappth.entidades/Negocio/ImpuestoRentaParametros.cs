namespace bd.webappth.entidades.Negocio
{
    using System.ComponentModel.DataAnnotations;

    public partial class ImpuestoRentaParametros
    {
        [Key]
        public int IdImpuestoRentaParametros { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Fracci�n b�sica:")]
        public decimal FraccionBasica { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Exceso de hasta:")]
        public decimal ExcesoHasta { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Impuesto de fracci�n b�sica:")]
        public int? ImpuestoFraccionBasica { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Range(0, 100, ErrorMessage = "El porcentaje deber�a ser entre 0 y 100")]
        [Display(Name = "Porcentaje de impuesto de fracci�n excedente:")]
        public int PorcentajeImpuestoFraccionExcedente { get; set; }
    }
}
