using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using Client;

[TestClass]
public class UnitTest
{
    [TestMethod]
    public void Add_Test()
    {
        ProcessingData pd = new ProcessingData();
        Assert.Equal(1, pd.addClients());
    }

    [TestMethod]
    public void Get_Test()
    {
        ProcessingData pd = new ProcessingData();
        Assert.Equal(1, pd.GetMessage());
    }

    [TestMethod]
    public void Broadcast_Test()
    {
        ProcessingData pd = new ProcessingData();
        Assert.Equal(1, pd.Broadcast());
    }
}