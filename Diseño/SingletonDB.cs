using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Diseño.DAL;

namespace Diseño
{
    public class SingletonDB
    {
        private static volatile DondeInviertoContext db;
        private static object syncRoot = new Object();

        private SingletonDB()
        {
            db = new DondeInviertoContext();
        }

        public static DondeInviertoContext Instance
        {
            get
            {
                if (db == null)
                {
                    lock (syncRoot)
                    {
                        if (db == null)
                            db = new DondeInviertoContext();
                    }
                }

                return db;
            }
        }
    }
}