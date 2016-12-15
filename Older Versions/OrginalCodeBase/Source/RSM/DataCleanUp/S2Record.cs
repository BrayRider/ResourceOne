using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RSM.Support.IO.Csv;

namespace DataCleanUp
{
    class ImageFile
    {
        string _fileName;
        string _first;
        string _last;
        public ImageFile(string filename)
        {
            _fileName = filename;
        }

        public string Filename {get {return _fileName;}}
        public string FirstInitial 
        {
            get
            {
                try
                {
                    if(_first == null)
                        _first =  _fileName.Split('_')[0].ToUpper();

                    return _first;
                }
                catch(Exception)
                {
                    return string.Empty;
                }
            }
        }

        public string LastName 
        {
            get
            {
                try
                {
                    if (_last == null)
                        _last = _fileName.Split('_')[1].ToUpper();

                    return _last;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

    }

    class S2Record
    {
        public string APICommand;
        public string FirstName;
        public string LastName;
        public string MiddleName;
        public string PictureFilename;
        public string PersonID;
        public string UDF1;
        public string UDF2;
        public string UDF3;
        public string UDF4;
        public string UDF5;
        public string UDF6;
        public string UDF7;
        public string UDF8;
        public string UDF9;
        public string UDF10;
        public string UpperLast;
        public string FirstInitial;
        public ImageFile Image;


        public string ToCSV()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                                  APICommand, FirstName, MiddleName, LastName, PictureFilename, PersonID,
                                  UDF1, UDF2, UDF3, UDF4, UDF5, UDF6, UDF7, UDF8, UDF9, UDF10);
        }

        public S2Record(CsvReader rdr)
        {
            APICommand = rdr[0];
            FirstName = rdr[1];
            LastName = rdr[3];
            UpperLast = LastName.ToUpper();
            try
            {
                FirstInitial = FirstName.Substring(0, 1).ToUpper();
            }
            catch (Exception)
            {
                FirstInitial = "";
            }
            MiddleName = rdr[2];
            PictureFilename = rdr[4];
            PersonID = rdr[5];
            UDF1 = rdr[6];
            UDF2 = rdr[7];
            UDF3 = rdr[8];
            UDF4 = rdr[9];
            UDF5 = rdr[10];
            UDF6 = rdr[11];
            UDF7 = rdr[12];
            UDF8 = rdr[13];
            UDF9 = rdr[14];
            UDF10 = rdr[15];

            Image = new ImageFile(PictureFilename);

        }
    }
}
