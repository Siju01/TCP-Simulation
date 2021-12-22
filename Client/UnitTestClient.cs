using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Client;

[TestClass]
public class UnitTest
{
    [TestMethod]
    public void Check_Test()
    {
        CheckM cm = new CheckM();
        Assert.Equal(1, cm.Check());
    }
}


[TestMethod]
public void Main_Test()
{
    Main m = new Main();
    Assert.Equal(1, m.Main());
}