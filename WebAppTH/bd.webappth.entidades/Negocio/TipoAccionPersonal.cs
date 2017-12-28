namespace bd.webappth.entidades.Negocio
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
     
    public partial class TipoAccionPersonal
    {
        [Key]
        public int IdTipoAccionPersonal { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Tipo de acci�n de personal:")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El {0} no puede tener m�s de {1} y menos de {2}")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "N�mero D�as M�nimo:")]
        public int NDiasMinimo { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "N�mero D�as M�ximo:")]
        public int NDiasMaximo { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "N�mero Horas M�nimo:")]
        public int NHorasMinimo { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "N�mero Horas M�ximo:")]
        public int NHorasMaximo { get; set; }
        
        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "D�as H�biles:")]
        public bool DiasHabiles { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Imputable Vacaciones:")]
        public bool ImputableVacaciones { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Proceso N�mina:")]
        public bool ProcesoNomina { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Es Responsable TH:")]
        public bool EsResponsableTH { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Matriz:")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El {0} no puede tener m�s de {1} y menos de {2}")]
        public string Matriz { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Descripci�n:")]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "El {0} no puede tener m�s de {1} y menos de {2}")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Genera Acci�n Personal:")]
        public bool GeneraAccionPersonal { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Modifica Distributivo:")]
        public bool ModificaDistributivo { get; set; }

        [Required(ErrorMessage = "Debe introducir {0}")]
        [Display(Name = "Estado Tipo Accion Personal:")]
        public int IdEstadoTipoAccionPersonal { get; set; }
        public virtual EstadoTipoAccionPersonal EstadoTipoAccionPersonal { get; set; }

        //Propiedades Virtuales Referencias a otras clases

        public virtual ICollection<AccionPersonal> AccionPersonal { get; set; }
    }
}
