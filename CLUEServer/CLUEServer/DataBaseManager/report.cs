
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace DataBaseManager
{

using System;
    using System.Collections.Generic;
    
public partial class report
{

    public int noReport { get; set; }

    public string reporter { get; set; }

    public string reported { get; set; }

    public string comment { get; set; }

    public int type { get; set; }

    public System.DateTime reportDate { get; set; }



    public virtual gamer gamer { get; set; }

    public virtual gamer gamer1 { get; set; }

}

}
