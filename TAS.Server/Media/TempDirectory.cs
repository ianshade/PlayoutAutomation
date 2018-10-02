﻿using System;
using System.Configuration;
using System.IO;
using TAS.Common;
using TAS.Common.Interfaces;

namespace TAS.Server.Media
{
    public class TempDirectory: MediaDirectoryBase
    {
        public TempDirectory()
        {
            Folder = ConfigurationManager.AppSettings["TempDirectory"];
            SweepStaleMedia();
        }

        public override void RemoveMedia(IMedia media)
        {
            throw new NotImplementedException();
        }

        public override IMedia CreateMedia(IMediaProperties media)
        {
            return new TempMedia(this, media);
        }



        private void SweepStaleMedia()
        {
            foreach (string fileName in Directory.GetFiles(Folder))
                try
                {
                    File.Delete(fileName);
                }
                catch
                {
                    // ignored
                }
        }


    }
}
