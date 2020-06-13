﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServerApp.Models.BindingTargets
{
    public class SupplierData
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string State { get; set; }

        public Supplier Supplier
        {
            get
            {
                return new Supplier
                {
                    Name = Name,
                    City = City,
                    State = State
                };
            }
        }
    }
}