﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SearchedTerm : EntityBase
    {

        public string Term { get; set; }
        public int Count { get; set; }

    }
}
