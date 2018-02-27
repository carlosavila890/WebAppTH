namespace bd.webappth.entidades.Negocio
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;


    public partial class EspecificidadExperiencia
    {
        [Key]
        public int IdEspecificidadExperiencia { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Especificaci�n de experiencia:")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "El {0} no puede tener m�s de {1} y menos de {2}")]
        public string Descripcion { get; set; }

        //Propiedades Virtuales Referencias a otras clases

        public virtual ICollection<ExperienciaLaboralRequerida> ExperienciaLaboralRequerida { get; set; }
    }
}
