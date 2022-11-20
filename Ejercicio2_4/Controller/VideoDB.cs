using Ejercicio2_4.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio2_4.Controller
{
    public class VideoDB
    {
        readonly SQLiteAsyncConnection db;

        public VideoDB(string dbpath)
        {
            db = new SQLiteAsyncConnection(dbpath);
            db.CreateTableAsync<Video>();
        }

        public Task<int> guardarVideo(Video video)
        {
            if (video.id != 0)
            {
                return db.UpdateAsync(video);
            }
            else
            {
                return db.InsertAsync(video);
            }
        }

        public Task<List<Video>> obtenerVideos()
        {
            return db.Table<Video>().ToListAsync();
        }

        public Task<Video> obtenerVideo(int vId)
        {
            return db.Table<Video>()
                .Where(i => i.id == vId)
                .FirstOrDefaultAsync();
        }

        public Task<int> borrarVideo(Video video)
        {
            return db.DeleteAsync(video);
        }
    }
}
