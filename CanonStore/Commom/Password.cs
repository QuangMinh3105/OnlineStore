using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanonStore.Commom
{
    public class Password
    {
        public string Encode(string password)
        {
            try
            {
                byte[] EncDataByte = new byte[password.Length];
                EncDataByte = System.Text.Encoding.UTF8.GetBytes(password);
                string EncrytedData = Convert.ToBase64String(EncDataByte);
                return EncrytedData;
            }catch(Exception ex)
            {
                throw new Exception("Error in Encode" + ex.Message);
            }
        }
    }
}