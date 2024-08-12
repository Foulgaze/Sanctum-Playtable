﻿using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CardInfo
{
    [Name("name")]
    public string name { get; set; } = "";

    [Name("setCode")]
    public string setCode { get; set; } = "";
    [Name("number")]
    public string cardNumber { get; set; } = "";
    [Name("power")]
    public string power { get; set; } = "";
    [Name("toughness")]

    public string toughness { get; set; } = "";
    [Name("manaCost")]
    public string manaCost { get; set; } = "";

    [Name("text")]
    public string text { get; set; } = "";
    [Name("type")]
    public string type { get; set; } = "";
    [Name("layout")]
    public string layout { get; set; } = "";
    [Name("uuid")]
    public string uuid { get; set; } = "";
    [Name("otherFaceIds")]
    public string otherFace { get; set; } = "";
}
