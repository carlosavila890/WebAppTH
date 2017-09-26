namespace bd.webappth.entidades.Negocio
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;


    public partial class EscalaEvaluacionTotal
    {
        [Key]
        public int IdEscalaEvaluacionTotal { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Nombre:")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El {0} no puede tener m�s de {1} y menos de {2} caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Descripci�n:")]
        [DataType(DataType.Text)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Porciento inicial:")]
        [Range(0, 100, ErrorMessage = "El {0} no puede ser m�s de {2} ni menos de {1}")]
        [DisplayFormat(DataFormatString = "{0:0.00}%", ApplyFormatInEditMode = false)]
        public decimal?  PorcientoDesde { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Porciento final:")]
        [Range(0, 100, ErrorMessage = "El {0} no puede ser m�s de {2} ni menos de {1} caracteres")]
        [DisplayFormat(DataFormatString = "{0:0.00}%", ApplyFormatInEditMode = false)]
        public decimal? PorcientoHasta { get; set; }

        public virtual ICollection<Eval001> Eval001 { get; set; }   
    }
}
