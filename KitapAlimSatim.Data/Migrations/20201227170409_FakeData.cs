using KitapAlimSatim.Data.Entities;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KitapAlimSatim.Data.Migrations
{
    public partial class FakeData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kayıtlı kullanıcılar
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Password", "Name", "IsAdmin" },
                values: new object[]
                {
                    1,
                    "g161210553@sakarya.edu.tr",
                    "123",
                    "Bahramullah Arayan",
                    true
                });
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Password", "Name", "IsAdmin" },
                values: new object[]
                {
                    2,
                    "g181210370@sakarya.edu.tr",
                    "123",
                    "Burhan Aydemir",
                    true
                });
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "Password", "Name", "IsAdmin" },
                values: new object[]
                {
                    3,
                    "g171210382@sakarya.edu.tr",
                    "123",
                    "Abdülhamid Sencer KILIÇ",
                    true
                });
            // Kayıtlı kitaplar
            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "Id", "Name", "Author", "FileName", "Description", "Publisher" },
                values: new object[]
                {
                    1,
                    "Hayaletin Çırağı",
                    "Joseph Delaney",
                    "hayaletin-ciragi.jpg",
                    "Hayaletler, cadılar, hortlaklar ve karanlık… Bir solukta okuyacağınız, Hampshire Book Award ödülünü alan Wardstone Günlükleri serisinin ilk kitabı Hayaletin Çırağı, sinemalarda izleyicilerin tüylerini diken diken etmeye hazırlanıyor… Bu kitabı okurken sayfaları ne kadar hızlı çevirdiğinize siz de şaşıracaksınız.\n“Tüyler ürpertici varlıklarla yakınlaşmayı seven okurların arayışları sona erdi.” -Kirkus Reviews 'Hem çocuklar hem yetişkinler için sürükleyici… çok güzel yazılmış.' The Good Book Guide",
                    "Tudem Yayınları"
                }
                );
            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "Id", "Name", "Author", "FileName", "Description", "Publisher" },
                values: new object[]
                {
                    2,
                    "Olasılıksız",
                    "Adam Fawer",
                    "olasiliksiz.jpg",
                    "Amerikalı yazar Adam Fewer’ın 2005 yılında yayınladığı “Olasılıksız”, tüm dünyada büyük yankı uyandıran bir başyapıt niteliği taşıyor. “İnsan, beyninin ne kadarını kullanabilir ki?” sorusuna yanıtların arandığı bu kitap, okuyucuları matematiksel düşüncenin ve bilimin etrafında topluyor. Eserde işlenen konu, Laplace’in şeytanı teorisi üzerinde dururken, aslında hiçbir şeyin şans eseri olmadığını ve geçmişteki olayların etkileşimi ile bu anın yaşanabileceğini gözler önüne seriyor. Zekice oluşturulmuş bir kurgu ile karşı karşıya kalacağınız bu kitap, sizi içerisinden çıkamayacağınız bir olasılıklar zincirine davet ediyor.",
                    "April Yayıncılık"
                }
                );
            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "Id", "Name", "Author", "FileName", "Description", "Publisher" },
                values: new object[]
                {
                    3,
                    "Dijital Kale",
                    "Dan Brown",
                    "dijital-kale.jpg",
                    "Ulusal Güvenlik Teşkilatı dünyanın kaderini değiştirecek ve dijital ortamdaki tüm şifreli metinleri bilecek özel bir bilgisayar üretir. Ne var ki, günün birinde bu özel bilgisayar karşılaştığı esrarengiz bir şifreyi çözemez. Ve kriptoloji uzmanı, zeki ve güzel Susan Fletcher göreve çağrılır. Genç kadın korkunç bir gerçekle yüzleşir. Silahlarla ya da bombalarla değil, Amerika Birleşik Devletleri'nin en güçlü haber alma örgütü olan Ulusal Güvenlik Teşkilatı çözülemez bir şifreyle rehin alınmıştır.\nSırlar ve yalanlar fırtınasına yakalanan Fletcher inandığı teşkilatı kurtarma savaşı verir. Dörtbir yandan ihanete uğrayan güzel kadın yalnızca ülkesini değil, kendi canını ve sevdiği erkeği de kurtarmaya çalışır...",
                    "Altın Kitaplar"
                }
                );
            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "Id", "Name", "Author", "FileName", "Description", "Publisher" },
                values: new object[]
                {
                    4,
                    "Harry Potter ve Felsefe Taşı",
                    "J. K. Rowling",
                    "harry-potter.jpg",
                    "“Harry, elleri titreyerek zarfı çevirince mor balmumundan bir mühür gördü; bir arma  koca bir ‘H’ harfinin çevresinde bir aslan, bir kartal, bir porsuk, bir de yılan.”Harry Potter sıradan bir çocuk olduğunu sanırken, bir baykuşun getirdiği mektupla yaşamı değişir: Başvurmadığı halde Hogwarts Cadılık ve Büyücülük Okulu’na kabul edilmiştir. Burada birbirinden ilginç dersler alır, iki arkadaşıyla birlikte maceradan maceraya koşar. Yaşayarak öğrendikleri sayesinde küçük yaşta becerikli bir büyücü olup çıkar.",
                    "Yapı Kredi Yayınları"
                }
                );
            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "Id", "Name", "Author", "FileName", "Description", "Publisher" },
                values: new object[]
                {
                    5,
                    "Deli Kurt",
                    "Hüseyin Nihal Atsız",
                    "deli-kurt.jpg",
                    "\"Deli Kurt\", Osmanlı tarihinde Yıldırım Bayazıd'dan sonra \"Şehzadeler Kavgası\" diye anılan devrin tarihî bir romanıdır. Bir bakıma göre de \"Bozkurtlar\"da başlayan Orta Asya'daki hayat kavgasının yeni vatan Anadolu'da devamıdır. Şehzadeler arasında süren ve tafsilâtı henüz yeterince aydınlanmamış bulunan çarpışmada Yıldırım'ın oğulları hayat ve taht mücadelesinin hem kahramanca, hem şairane, hem de sefîhane bir örneğini vermişler ve birbiri ardınca hayata veda ederek meydanı içlerinden birisine bırakmışlardır. Bunlar arasında en talihsizi ve hayatı en az bilineni İsa Çelebi'dir. Deli Kurt, İsa Çelebi'nin meçhul bir oğlunun dramıdır. Bu dram daha sonraki asırlarda daha büyük bir şiddetle sürüp gidecek ve yüzlerce şehzadenin hayatına mal olacaktır. Romanda görülen parlak bakışlı, gözlerine bakılamayan kız, hayalî bir tip değildir. Zamanımızda Muğla köylerinden birinde böyle bir kız yaşamıştır ve belki de hâlâ yaşamaktadır. Roman yazarı, bu parlak ve büyülü bakışları beş yüz yıl öncesine götürmekle esere çeşni vermekten başka bir şey yapmamıştır.",
                    "Ötüken Neşriyat"
                }
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1);
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2);
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3);
            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: 1);
            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: 2);
            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: 3);
            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: 4);
            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
