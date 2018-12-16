using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace FormSubmit.Models
{
    public class FormDataContext:DbContext
    {
        public FormDataContext() : base("UserDataConnection") { }
        public DbSet<FormModel> FormTable { get; set; }
    }
}