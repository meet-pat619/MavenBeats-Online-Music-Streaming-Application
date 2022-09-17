using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMusicMVC.Data;
using DemoMusicMVC.Models;
using Microsoft.AspNetCore.Hosting;
using DemoMusicMVC.ViewModel;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DemoMusicMVC.Controllers
{
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public SongsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin,Customer,PremiumCustomer")]
        // GET: Songs
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            IList<string> role =await _userManager.GetRolesAsync(user);
            if(role != null && (role[0] == "Admin" || role[0] == "PremiumCustomer"))
            {
                return View(await _context.songs.ToListAsync());
            }
            return View("VisitorPage", await _context.songs.ToListAsync());
        }
        [Authorize(Roles = "Admin,Customer,PremiumCustomer")]
        // GET: Songs
        public async Task<IActionResult> VisitorPage()
        {            
            return View(await _context.songs.ToListAsync());
        }
        [Authorize(Roles = "Admin,Customer,PremiumCustomer")]
        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {            
            var user = await _userManager.GetUserAsync(HttpContext.User);
            IList<string> role = await _userManager.GetRolesAsync(user);
            if (role != null && (role[0] == "Admin" || role[0] == "PremiumCustomer"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                var song = await _context.songs
                    .FirstOrDefaultAsync(m => m.songId == id);
                if (song == null)
                {
                    return NotFound();
                }

                return View(song);
            }
            else
            {
                if (id == null)
                {
                    return NotFound();
                }

                var song = await _context.songs
                    .FirstOrDefaultAsync(m => m.songId == id);
                if (song == null)
                {
                    return NotFound();
                }

                return View("PlaySong",song);
            }
        }
        [Authorize(Roles = "Admin,PremiumCustomer")]
        // GET: Songs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,PremiumCustomer")]
        public async Task<IActionResult> Create(SongViewModel uploadedSong)
        {
            string pathForPhoto = null, pathForSong = null;
            if (ModelState.IsValid)
            {
                if (uploadedSong.song != null)
                {
                    string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "Songs");
                    pathForSong = Guid.NewGuid().ToString() + "_" + uploadedSong.song.FileName;
                    string filePath = Path.Combine(uploadFolder, pathForSong);
                    uploadedSong.song.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                if (uploadedSong.photo != null)
                {
                    string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "Photos");
                    pathForPhoto = Guid.NewGuid().ToString() + "_" + uploadedSong.photo.FileName;
                    string filePath = Path.Combine(uploadFolder, pathForPhoto);
                    uploadedSong.photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                if(uploadedSong.song != null && uploadedSong.photo != null) { 
                    Song s = new Song
                    {
                        songName = uploadedSong.songName,
                        photoPath = pathForPhoto,
                        songPath = pathForSong
                    };
                    _context.Add(s);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(uploadedSong);
        }
        [Authorize(Roles = "Admin,PremiumCustomer")]
        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            string spath = song.songPath;
            string ppath = song.photoPath;
            string webRoot = hostingEnvironment.WebRootPath;
            if(System.IO.Directory.Exists(webRoot + "/Photos/") && System.IO.Directory.Exists(webRoot + "/Songs/"))
            {
                FormFile pfile = null, sfile = null;
                /*if(System.IO.File.Exists(webRoot+"/Photos/"+ppath))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(webRoot + "/Photos/" + ppath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }                    
                    memory.Position = 0;
                    pfile = new FormFile(memory,0,memory.Length,ppath,ppath);
                    memory.Close();
                }
                if (System.IO.File.Exists(webRoot + "/Songs/" + spath))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(webRoot + "/Songs/" + spath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    sfile = new FormFile(memory, 0, memory.Length, spath, spath);
                    memory.Close();
                }*/
                SongViewModel s = new SongViewModel()
                {
                    songId = song.songId,
                    songName = song.songName,
                    song = sfile,
                    photo = pfile
                };
                return View(s);
            }
            else
            {
                return NotFound();
            }
            
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,PremiumCustomer")]
        public async Task<IActionResult> Edit(int id, SongViewModel _song)
        {
            string pathForPhoto = null, pathForSong = null;
            if (id != _song.songId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_song.song != null)
                    {
                        string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "Songs");
                        pathForSong = Guid.NewGuid().ToString() + "_" + _song.song.FileName;
                        string filePath = Path.Combine(uploadFolder, pathForSong);
                        _song.song.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                    if (_song.photo != null)
                    {
                        string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "Photos");
                        pathForPhoto = Guid.NewGuid().ToString() + "_" + _song.photo.FileName;
                        string filePath = Path.Combine(uploadFolder, pathForPhoto);
                        _song.photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                    var song = await _context.songs.FirstOrDefaultAsync(m => m.songId == id);
                    if (song == null)
                    {
                        return NotFound();
                    }
                    song.songName = _song.songName;
                    song.songPath = pathForSong;
                    song.photoPath = pathForPhoto;
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(_song.songId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(_song);
        }
        [Authorize(Roles = "Admin,PremiumCustomer")]
        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.songs
                .FirstOrDefaultAsync(m => m.songId == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,PremiumCustomer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.songs.FindAsync(id);
            _context.songs.Remove(song);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> Convert()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await _userManager.AddToRoleAsync(user, "PremiumCustomer");
            await _userManager.RemoveFromRoleAsync(user, "Customer");
            await _userManager.UpdateAsync(user);            
            return View();            
        }
        private bool SongExists(int id)
        {
            return _context.songs.Any(e => e.songId == id);
        }
    }
}
