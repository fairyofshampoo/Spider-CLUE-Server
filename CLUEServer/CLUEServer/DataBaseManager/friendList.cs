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
    
    public partial class friendList
    {
        public int noList { get; set; }
        public string friend { get; set; }
        public string gamertag { get; set; }
    
        public virtual gamer gamer { get; set; }
        public virtual gamer gamer1 { get; set; }
    }
}
