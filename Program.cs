

using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

public class DirWalker
{

    
    int skippedcount = 0;
    int totalValidRows = 0;
    String cur_dir = Directory.GetCurrentDirectory();





    
    public void walk(String path)
    {
        string outputCsvFilePath = Path.Join(Directory.GetParent(cur_dir).Parent.Parent.FullName.ToString(), "\\ProgAssign1\\output\\output.csv");

        string outputTextFilePath = Path.Join(Directory.GetParent(cur_dir).Parent.Parent.FullName.ToString(), "\\ProgAssign1\\logs\\log.txt");

        string[] list = Directory.GetDirectories(path);
       




        if (list == null) return;


        foreach (string dirpath in list)
        {
            if (Directory.Exists(dirpath))
            {
                walk(dirpath);
                
            }
        }
        string[] fileList = Directory.GetFiles(path);


        processData(fileList,path);
        
        
    }

    private void processData(String[] fileList, String path)
    {
        foreach (string filepath in fileList)
        {
            string formattedDate = null;
            string[] parts = path.Split('\\');

            if (parts.Length >= 3)
            {
                string day = parts[parts.Length - 1];
                string month = parts[parts.Length - 2];
                string year = parts[parts.Length - 3];


                formattedDate = $"{year}/{month.ToString().PadLeft(2, '0')}/{day.ToString().PadLeft(2, '0')} ";
                formattedDate = formattedDate.Replace("\"", "");

                
            }
            if (filepath.Contains(".csv"))
            {

                
                var reader = new StreamReader(filepath);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)

                {
                    MissingFieldFound = null,
                    HasHeaderRecord = true
                };

                var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null,
                    HasHeaderRecord = true
                });
                var records = csv.GetRecords<Customer>().ToList();
                string outputCsvFilePath = Path.Join(Directory.GetParent(cur_dir).Parent.Parent.FullName.ToString(), "\\ProgAssign1\\output\\output.csv");

                string outputTextFilePath = Path.Join(Directory.GetParent(cur_dir).Parent.Parent.FullName.ToString(), "\\ProgAssign1\\logs\\log.txt");

                using (var errorFileWriter = new StreamWriter(outputTextFilePath, true))

                using (var txtWriter = new CsvWriter(errorFileWriter, config))
                using (var writer = new StreamWriter(outputCsvFilePath, true))
                using (var csvWriter = new CsvWriter(writer, config))

                    foreach (var record in records)
                    {
                        if (dataCheck(record))
                        {

                            csvWriter.WriteRecord(record);
                            csvWriter.WriteField(formattedDate);
                            csvWriter.NextRecord();
                            csvWriter.Flush();
                            totalValidRows++;
                        }
                        else
                        {
                            skippedcount++;  
                            txtWriter.WriteRecord(record);
                            txtWriter.WriteField(formattedDate);
                            txtWriter.NextRecord();
                            txtWriter.Flush();


                        }

                    }


            }
        }
    }

    private Boolean dataCheck(Customer record)
    {
        if (record.FirstName == "" || record.LastName == "" || record.PhoneNumber == "" || record.Province == "" || record.StreetNumber == "" ||
            record.Street == "" || record.City == "" || record.Province == "" || record.PostalCode == "" || record.Country == "")
        {
            return false;
        }
        return true;

    }

    public class Customer
    {
        [Name("First Name")]
        public string FirstName { get; set; }

        [Name("Last Name")]
        public string LastName { get; set; }

        [Name("Street Number")]
        public string StreetNumber { get; set; }

        [Name("Street")]
        public string Street { get; set; }

        [Name("City")]
        public string City { get; set; }

        [Name("Province")]
        public string Province { get; set; }

        [Name("Postal Code")]
        public string PostalCode { get; set; }

        [Name("Country")]
        public string Country { get; set; }

        [Name("Phone Number")]
        public string PhoneNumber { get; set; }

        [Name("email Address")]
        public string EmailAddress { get; set; }

      



    }

    public class CustomerForWriting
    {
        [Name("First Name")]
        public string FirstName { get; set; }

        [Name("Last Name")]
        public string LastName { get; set; }

        [Name("Street Number")]
        public string StreetNumber { get; set; }

        [Name("Street")]
        public string Street { get; set; }

        [Name("City")]
        public string City { get; set; }

        [Name("Province")]
        public string Province { get; set; }

        [Name("Postal Code")]
        public string PostalCode { get; set; }

        [Name("Country")]
        public string Country { get; set; }

        [Name("Phone Number")]
        public string PhoneNumber { get; set; }

        [Name("email Address")]
        public string EmailAddress { get; set; }

        [Name("Date")]
        public string date { get; set; }
    }

    public CustomerForWriting ConvertCustomerToWriting(Customer customerForReading,string date)
    {
        var CustomerForWriting = new CustomerForWriting
        {
            FirstName = customerForReading.FirstName,
            LastName = customerForReading.LastName,
            StreetNumber = customerForReading.StreetNumber,
            Street = customerForReading.Street,
            City = customerForReading.City,
            Province = customerForReading.Province,
            PostalCode = customerForReading.PostalCode,
            Country = customerForReading.Country,
            PhoneNumber = customerForReading.PhoneNumber,
            EmailAddress = customerForReading.EmailAddress,
            date = date
        };

        return CustomerForWriting;
    }
     public static void Main(String[] args)
    {
        DateTime startTime = DateTime.Now;
        DirWalker fw = new DirWalker();
        String errorMessage = null;
        String cur_dir = Directory.GetCurrentDirectory();
        string outputCsvFilePath = Path.Join(Directory.GetParent(cur_dir).Parent.Parent.FullName.ToString(), "\\ProgAssign1\\output\\output.csv");

        string outputTextFilePath = Path.Join(Directory.GetParent(cur_dir).Parent.Parent.FullName.ToString(), "\\ProgAssign1\\logs\\log.txt");
        try
        {
            
            if (File.Exists(outputCsvFilePath))
            {
                File.Delete(outputCsvFilePath);
            }
            if (File.Exists(outputTextFilePath))
            {
                File.Delete(outputTextFilePath);
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)

            {
                MissingFieldFound = null,
                HasHeaderRecord = true
            };
            using (var writer = new StreamWriter(outputCsvFilePath, true))
            using (var csvWriter = new CsvWriter(writer, config))
            {
                csvWriter.WriteHeader<CustomerForWriting>();
                csvWriter.NextRecord();
            }

            
            Console.WriteLine("Enter Path : ");
            var dirPath = Console.ReadLine();

            fw.walk(dirPath);
        }
        catch (FileNotFoundException)
        {
            errorMessage = "The file or directory cannot be found.";
        }
        catch (DirectoryNotFoundException)
        {
            errorMessage = "The file or directory cannot be found.";
        }
        catch (DriveNotFoundException)
        {
            errorMessage = "The drive specified in 'path' is invalid.";
        }
        catch (PathTooLongException)
        {
            errorMessage = "'path' exceeds the maxium supported path length.";
        }
        catch (UnauthorizedAccessException)
        {
            errorMessage = "You do not have permission to create this file.";
        }
        catch (IOException e) when ((e.HResult & 0x0000FFFF) == 32)
        {
            Console.WriteLine("There is a sharing violation.");
        }
        catch (IOException e) when ((e.HResult & 0x0000FFFF) == 80)
        {
            errorMessage = "The file already exists.";
        }
        catch (IOException e)
        {
            errorMessage = $"An exception occurred:\nError code: " +
                              $"{e.HResult & 0x0000FFFF}\nMessage: {e.Message}";
        }
        catch (Exception e)
        {
            errorMessage = ("General exception occured the error message is " + e.Message);
        }
        finally
        {
            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;
            Console.WriteLine(errorMessage);
            Console.WriteLine("The elapsed time is " + elapsedTime);
            Console.WriteLine("The total valid rows is " + fw.totalValidRows);
            Console.WriteLine("The total invalid rows is " + fw.skippedcount);
            Console.WriteLine("Output CSV Path : "+ outputCsvFilePath);
            Console.WriteLine("Log file Path : " + outputTextFilePath);
            using (StreamWriter logWriter = File.AppendText(outputTextFilePath))
            {
                if (errorMessage != null)
                {
                    logWriter.WriteLine(errorMessage);
                }
                logWriter.WriteLine("The elapsed time is " + elapsedTime);
                logWriter.WriteLine("The total valid rows is " + fw.totalValidRows);
                logWriter.WriteLine("The total invalid rows is " + fw.skippedcount);

            }
        }

    }




    

}