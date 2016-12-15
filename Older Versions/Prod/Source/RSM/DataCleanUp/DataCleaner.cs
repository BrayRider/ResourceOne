using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RSM.Support.IO.Csv;
using System.IO;
using System.Windows.Forms;

namespace DataCleanUp
{
    class DataCleaner
    {
        string _inputFile;
        List<S2Record> _records;
        List<ImageFile> _imageFiles;
        List<S2Record> _mismatchedRecords;
        Dictionary<string, string> _hotStampMap;
        //List<string> _IDMap;

        int _dupCount;
        int _mismatached;
        int _rematched;
        int _rmTooMany;
        int _rmNoMatch;

        public int TooMany { get { return _rmTooMany; } }
        public int NoMatch { get { return _rmNoMatch; } }
        public int RecordCount
        {
            get {
                if (_records != null)
                    return _records.Count;
                return 0;
            }
        }
        public int DupCount
        {
            get
            {
                return _dupCount;
            }
        }
        public int MismatchedCount { get { return _mismatached; } }
        public int RematchedCount { get { return _rematched; } }
        public DataCleaner(string inputFile)
        {

            _inputFile = inputFile;
            
        }

        public int FindDuplicates()
        {
            var dups = (from r in _records where r.UDF6.Length > 6 select r).ToArray();
            _dupCount = dups.Length;

            var o = (from r in _records
                         where  r.UDF6.Length > 5 && r.UDF9.Length > 5 && r.UDF6.Substring(1, 5) != r.UDF9.Substring(1, 5) 
                         select r).ToArray();


            foreach (var dup in dups)
            {
                dup.APICommand = "DELETE";
                try
                {
                    var other = (from r in _records
                                 where r.UDF6.Substring(1, 5) != r.UDF9.Substring(1,5) &&
                                       r.LastName == dup.LastName &&
                                       r.FirstName == dup.FirstName &&
                                       r.MiddleName == dup.MiddleName &&
                                       r.UDF1 == dup.UDF1
                                 select r).Single();
                    other.PictureFilename = dup.PictureFilename;
                    other.APICommand = "MODIFY";
                }
                catch (Exception e)
                {
                    var s = e.ToString();
                }
            }


            


            //var noUDF6 = (from r in _records where r.UDF6.Length < 1 select r).ToArray();
            //foreach (var rec in noUDF6)
            //{
            //    try
            //    {
            //        var others = (from r in _records
            //                      where (r.UDF6.Length > 1) &&
            //                            (r.FirstName == rec.FirstName) &&
            //                            (r.LastName == rec.LastName) &&
            //                            (r.MiddleName == rec.MiddleName) &&
            //                            (r.UDF1 == rec.UDF1)
                                                      
            //                      select r).ToArray();
                    
            //        rec.APICommand = "DELETE";
            //        _dupCount++;
            //    }
            //    catch (Exception)
            //    {
            //    }

            //}


            return _dupCount;
        }

        public int FindMismatchedImages()
        {
            var noImageRecs = (from r in _records where r.PictureFilename.Length == 0 && (r.APICommand != "DELETE")  select r).ToArray();

            foreach( var nir in noImageRecs)
            {
                try
                {
                 

                    var duped = (from r in _records where r.APICommand == "DELETE" &&  
                                                          r.FirstName == nir.FirstName && 
                                                          r.LastName == nir.LastName && 
                                                          r.MiddleName == nir.MiddleName && 
                                                          r.UDF3 == nir.UDF3 select r).Single();
                   
                    nir.PictureFilename = duped.PictureFilename;
                    nir.Image = duped.Image;
                    nir.APICommand = "MODIFY";
                }
                catch(Exception)
                {}
            }


            _mismatchedRecords = (from r in _records where (r.APICommand != "DELETE") && (r.UpperLast != r.Image.LastName) select r).ToList();
            _mismatached = _mismatchedRecords.Count;

         
            return _mismatached;
        }


        public int FixMismatchedImages()
        {
            _rematched = 0;
            foreach (S2Record rec in _mismatchedRecords)
            {
                try
                {
                    var img = (from i in _imageFiles where (i.LastName == rec.UpperLast) && (i.FirstInitial == rec.FirstInitial) select i).Single();
                    rec.PictureFilename = img.Filename;
                    rec.Image = img;
                    rec.APICommand = "MODIFY";
                    _rematched++;
                }
                catch (Exception e)
                {
                    if (e.Message == "Sequence contains no elements")
                    {
                        rec.UDF10 = "NO MATCH";
                        _rmNoMatch++;
                    }
                    else
                    {
                        rec.UDF10 = "TOO MANY";
                        _rmTooMany++;
                    }
                    rec.APICommand = "MODIFY";
                    rec.PictureFilename = string.Empty;
                }
            }

            return _rematched;
        }


        public void SaveFile()
        {
            FileStream stream = new FileStream("C:\\output.csv", FileMode.Create);
            TextWriter tw = new StreamWriter(stream);

            var final = (from r in _records where r.APICommand == "DELETE" || r.APICommand == "MODIFY" select r).ToArray();


            foreach (var rec in final)
            {
                tw.WriteLine(rec.ToCSV());
            }

            tw.Close();
            stream.Close();
        }

        public void LoadHotStampFile()
        {
            _hotStampMap = new Dictionary<string,string>();
            CsvReader rdr = new CsvReader(new StreamReader(_inputFile), true);
            //rdr.HasHeaders = true;
            using (rdr)
            {
                //rdr.ReadNextRecord();
                while (rdr.ReadNextRecord())
                {
                    var empID = rdr["Text9"];
                    var hotstamp = rdr["HotStampNum1"];
                    if(hotstamp != string.Empty)
                        _hotStampMap[empID] = hotstamp;
                }
            }


            FileStream stream = new FileStream("C:\\output_hotstamp.csv", FileMode.Create);
            TextWriter tw = new StreamWriter(stream);
            
            

            rdr = new CsvReader(new StreamReader("C:\\prj\\resourceone\\IDs.csv"), true);
            //rdr.HasHeaders = true;
            using (rdr)
            {
                //rdr.ReadNextRecord();
                while (rdr.ReadNextRecord())
                {
                    var empID = rdr["EmployeeID"];
                    var PersonID = rdr["PersonID"];
                    
                    if(_hotStampMap.ContainsKey(empID))
                    {
                         tw.WriteLine(string.Format("MODIFY,{0},{1},{2}", PersonID, _hotStampMap[empID], _hotStampMap[empID]) );
                    }

                    
                }
            }
            tw.Close();
            stream.Close();


            MessageBox.Show("Done");
        }

        public void LoadFile()
        {
            _dupCount = _rmTooMany = _rmNoMatch =  0;
            _mismatached = 0;
            _records = new List<S2Record>();
            _imageFiles = new List<ImageFile>();
            CsvReader rdr = new CsvReader(new StreamReader(_inputFile), false);

            using (rdr)
            {
                rdr.ReadNextRecord();
                while (rdr.ReadNextRecord())
                {
                    var rec = new S2Record(rdr);
                    _records.Add(rec);

                    if (rec.PictureFilename.Length > 0)
                    {
                        if (rec.PersonID != rec.UDF6)
                        {
                            if(rec.UpperLast != rec.Image.LastName)
                                _imageFiles.Add(new ImageFile(rdr[4]));
                        }
                        else
                        {
                            _imageFiles.Add(new ImageFile(rdr[4]));
                        }
                    }


                    
                        
                }
            }
        }
    }
}
