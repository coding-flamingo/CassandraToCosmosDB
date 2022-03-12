// See https://aka.ms/new-console-template for more information
using CassandraToAzureFiles.models;
using System.Text;
using System.Text.Json;

Console.WriteLine("Enter full path to your Cassandra File");
string fileContents = String.Empty;
string filepath = Console.ReadLine().Trim();
Console.WriteLine("Enter server name");
string serverName = Console.ReadLine().Trim();
Console.WriteLine("Enter Resource Group name");
string resourceGroup = Console.ReadLine().Trim();
Console.WriteLine("Enter Key Space name");
string keySpace = Console.ReadLine().Trim();
try
{
    using (var sr = new StreamReader(filepath))
    {
        fileContents = sr.ReadToEnd();
    }
}
catch (Exception e)
{
    Console.WriteLine("The file could not be read:");
    Console.WriteLine(e.Message);
    return;
}
if(string.IsNullOrEmpty(fileContents))
{
    Console.WriteLine("Error file is empty");
    return;
}
string[] tables = fileContents.Split("CREATE TABLE");
List< CassandraTableModel > azTables = new ();
foreach(string table in tables)
{
    if(!string.IsNullOrWhiteSpace(table))
    {
        string[] lines = table.Split("\n");
        CassandraTableModel newTable = new CassandraTableModel();
        newTable.Name = lines[0].Split("(")[0].Trim();
        for (int i = 1; i < lines.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(lines[i]))
            {
                if (lines[i].Contains("PRIMARY KEY"))
                {
                    string line = lines[i].Trim()
                        .Replace("PRIMARY KEY", "").TrimEnd(')')
                        .TrimEnd(')').Replace("(","");
                    foreach (string s in line.Split(","))
                    {
                        newTable.jsonContents.PartitionKeys.Add(new()
                        {
                            Name = s.Trim(),
                        });
                    }
                }
                else
                {
                    string line = lines[i].Trim().TrimEnd(',');
                    newTable.jsonContents.Columns.Add(new()
                    {
                        Name = line.Split(" ")[0],
                        Type = line.Split(" ")[1]
                    });
                }
            }
        }
        azTables.Add(newTable);
    }
}
string powershell = string.Empty;
foreach (CassandraTableModel table in azTables)
{
    string jsondata = JsonSerializer.Serialize(table.jsonContents);
    using (FileStream fs = File.Create("output\\" + table.Name + ".json"))
    {
        byte[] info = new UTF8Encoding(true).GetBytes(jsondata);
        fs.Write(info, 0, info.Length);
    }
    powershell += "az cosmosdb cassandra table create" +
        $" --account-name {serverName} --keyspace-name {keySpace}" +
        $" --name {table.Name} --resource-group {resourceGroup} --schema '@{table.Name}.json' \n";
}
using (FileStream fs = File.Create("output\\createTables.ps1"))
{
    byte[] info = new UTF8Encoding(true).GetBytes(powershell);
    fs.Write(info, 0, info.Length);
}