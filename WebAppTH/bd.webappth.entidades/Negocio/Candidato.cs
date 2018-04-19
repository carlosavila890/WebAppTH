namespace bd.webappth.entidades.Negocio
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Candidato
    {
        [Key]
        public int IdCandidato { get; set; }


        public int IdPersona { get; set; }
        public virtual Persona Persona { get; set; }

        //Propiedades Virtuales Referencias a otras clases
        public virtual ICollection<CandidatoConcurso> CandidatoConcurso { get; set; }

    }
}
