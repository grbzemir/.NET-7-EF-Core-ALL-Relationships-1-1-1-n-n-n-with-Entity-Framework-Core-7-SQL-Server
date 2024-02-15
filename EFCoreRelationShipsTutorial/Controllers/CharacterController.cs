using EFCoreRelationShipsTutorial.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRelationShipsTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {

        private readonly DataContext _context; // Veritabanı bağlantısı için gerekli olan nesne

        //Yapıcı metot
        public CharacterController(DataContext context)
        {
            _context = context;

        }

        [HttpGet] // Get isteği için kullanılır

        public async Task<ActionResult<List<Character>>> Get(int userId) // Kullanıcı id'sine göre karakterleri getirir
        {
            var characters = await _context.Characters.Where(c => c.UserId == userId)
                .Include(c => c.Weapon) // Silahları getirir
                .ToListAsync();// Veritabanındaki tüm karakterleri getirir

            return characters; // Karakterleri döndürür



        }

        [HttpPost] // Post isteği için kullanılır

        public async Task<ActionResult<List<Character>>> Create(CreateCharacterDto request) // Yeni karakter ekler
        {
            var user = await _context.Users.FindAsync(request.UserId); // Kullanıcıyı bulur

            if (user == null) // Kullanıcı yoksa
            {
                return NotFound("User not found"); // Kullanıcı bulunamadı hatası döndürür
            }

            var newCharacter = new Character
            {

                Name = request.Name,
                RpgClass = request.RpgClass,
                UserId = request.UserId

            };

            _context.Characters.Add(newCharacter); // Karakteri ekler
            await _context.SaveChangesAsync(); // Değişiklikleri kaydeder

            return await Get(newCharacter.UserId); // Karakteri döndürür

        }

        [HttpPost("waepon")] // Silah ekler

        public async Task<ActionResult<Character>>AddWeapon(AddWeaponDto request) // Silah ekler
        {
            var character = await _context.Characters.FindAsync(request.CharacterId); // Karakteri bulur

            if (character == null) // Karakter yoksa
            {
                return NotFound("Character not found"); // Karakter bulunamadı hatası döndürür
            }

            var weapon = new Weapon
            {
                Name = request.Name,
                Damage = request.Damage,
                CharacterId = request.CharacterId
            };

            _context.Weapons.Add(weapon); // Silahı ekler
            await _context.SaveChangesAsync(); // Değişiklikleri kaydeder

            return character; // Karakteri döndürür
        }

        [HttpPost("skill")] // Yetenek ekler

        public async Task<ActionResult<Character>> AddSkill(AddSkillDto request) // Yetenek ekler
        {
            var character = await _context.Characters
                .Where(c => c.Id == request.CharacterId) // Karakteri bulur
                .Include(c => c.Skills) // Yetenekleri getirir
                .FirstOrDefaultAsync(); // İlk karakteri getirir


            if (character == null) // Karakter yoksa
            {
                return NotFound("Character not found"); // Karakter bulunamadı hatası döndürür
            }

            var skill = await _context.Skills.FindAsync(request.CharacterId); // Yeteneği bulur
           
            if (skill == null) // Yetenek yoksa
            {
                return NotFound("Skill not found"); // Yetenek bulunamadı hatası döndürür
            }


            _context.Skills.Add(skill); // Yeteneği ekler
            await _context.SaveChangesAsync(); // Değişiklikleri kaydeder

            return character; // Karakteri döndürür
        }




    }

}