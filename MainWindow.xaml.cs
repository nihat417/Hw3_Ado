using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hw3_Ado;


public partial class MainWindow : Window
{

    DbConnection? conn = null;
    DbDataAdapter? adapter = null;
    IConfigurationRoot? configurationRoot = null;
    DbProviderFactory? providerFactory = null;
    DataSet? dataSet = null;
    string providerName = string.Empty;
    public MainWindow()
    {
        InitializeComponent();
        Configure();
    }


    private void Configure()
    {
        DbProviderFactories.RegisterFactory("System.Data.SqlClient", typeof(SqlClientFactory));

        string projectPath = Directory.GetCurrentDirectory().Split(@"bin\")[0];
        configurationRoot =new ConfigurationBuilder().SetBasePath(projectPath).AddJsonFile("appsettings.json").Build();

        providerName = "System.Data.SqlClient";
        providerFactory= DbProviderFactories.GetFactory(providerName);
        dataSet = new DataSet();
        conn = providerFactory.CreateConnection();

        conn.ConnectionString = configurationRoot.GetConnectionString(providerName);
        adapter = providerFactory.CreateDataAdapter();



        DataTableMapping mapping1 = new("Table", "Books");
        DataTableMapping mapping2 = new("Table1", "Authors");
        DataTableMapping mapping3 = new("Table2", "S_Cards");
        DataTableMapping mapping4 = new("Table3", "T_Cards");

        adapter.TableMappings.Add(mapping1);
        adapter.TableMappings.Add(mapping2);
        adapter.TableMappings.Add(mapping3);
        adapter.TableMappings.Add(mapping4);
    }


    private void TabCreate()
    {
        foreach (DataTable table in dataSet.Tables)
        {
            var tabit = new TabItem();
            tabit.Header = table.TableName;
            var dataGrid = new DataGrid();
            dataGrid.ItemsSource = table.AsDataView();
            tabit.Content = dataGrid;
            tab.Items.Add(tabit);
        }
    }

    private void execBtn_Click(object sender, RoutedEventArgs e)
    {
        using var command = conn.CreateCommand();
        command.CommandText = serachtxtbox.Text;
        command.Connection = conn;
        adapter.SelectCommand = command;
        if (tab.Items.Count > 1)
        {
            for (int i = tab.Items.Count - 1; i > 0; i--)
                tab.Items.RemoveAt(i);
        }
        dataSet.Tables.Clear();
        try
        {
            adapter.Fill(dataSet);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }


        TabCreate();
    }
}
