using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FilipinoFolkloreApp.Models;
namespace FilipinoFolkloreApp.Services
{
    public static class MedalHelper
    {
        public static List<Medals> Medals { get; } = new()
        {
            new Medals
            {
                MedalId = 1,
                MedalName = "Badge ng Alimango",
                MedalImagePath = "medals/badgealamatngalimango.png",
                isUnlocked = false,
                MedalDescription = "Basahin ang kwento at masagot nang tama ang tanong tungkol sa Alimango.\nPinapakita ang talino at stratehiya ng alimango sa kwento\r\n"
            },
            new Medals
            {
                MedalId = 2,
                MedalName = "Badge ng Bahaghari",
                MedalImagePath = "medals/badgealamatngbahaghari.png",
                isUnlocked = false,
                MedalDescription = "Makumpleto ang kwento at sagutin ang quiz tungkol sa bahaghari.\nIpinapakita ang ganda at kahalagahan ng bahaghari sa kwento\r\n"
            },
            new Medals
            {
                MedalId = 3,
                MedalName = "Badge ng Pinya",
                MedalImagePath = "medals/badgealamatngpinya.png",
                isUnlocked = false,
                MedalDescription = "Basahin ang kwento at piliin ang tamang aral tungkol sa pinya.\nPinapakita ang kabutihan at kasipagan ng karakter sa kwento\r\n"
            },
            new Medals
            {
                MedalId = 4,
                MedalName = "Badge ng Araw, Buwan, at Bituin",
                MedalImagePath = "medals/badgealamatngbuwanarawatmgabituin.png",
                isUnlocked = false,
                MedalDescription = "Makumpleto ang kwento at masagot nang tama ang quiz.\nIpinapakita ang pag-unawa sa pinagmulan ng araw, buwan, at mga bituin ayon sa alamat\r\n"
            },
            new Medals
            {
                MedalId = 5,
                MedalName = "Badge kung bakit may ulan",
                MedalImagePath = "medals/badgekungbakitmayulan.png",
                isUnlocked = false,
                MedalDescription = "Makumpleto ang kwento at masagot nang tama ang quiz tungkol sa ulan.\nIpinapakita ang pag-unawa sa pinagmulan ng ulan ayon sa alamat\r\n"
            },
            new Medals
            {
                MedalId = 6,
                MedalName = "Prinsipe ng Tapang Badge",
                MedalImagePath = "medals/badgebantugan.png",
                isUnlocked = false,
                MedalDescription = "Makakuha ng mataas na score sa quiz o decision choices.\nSi Bantugan ay bayani sa epikong Darangen ng mga Maranao. \r\n"
            },
            new Medals
            {
                MedalId = 7,
                MedalName = "Alamat ng Halawod Badge",
                MedalImagePath = "medals/badgehinilawod.png",
                isUnlocked = false,
                MedalDescription = "Makumpleto ang buong story sequence.\nAng epiko ay tungkol sa pakikipagsapalaran ng tatlong magkakapatid na demigod na bayani ng Panay. \r\n"
            },
            new Medals
            {
                MedalId = 8,
                MedalName = "Bayani ng Ibalong Badge",
                MedalImagePath = "medals/badgeibalon.png",
                isUnlocked = false,
                MedalDescription = "Makumpleto ang story at sagutan ang activity tungkol sa mga bayani.\nIpinapakita ng epiko ang kabayanihan nina Baltog at Handyong laban sa mga halimaw at pagsasaayos ng pamayanan."
            },
            new Medals
            {
                MedalId = 9,
                MedalName = "Lakbay-Puso Badge",
                MedalImagePath = "medals/badgemanimimbin.png",
                isUnlocked = false,
                MedalDescription = "Matapos basahin o makumpleto ang story ng Manimimbin.\nAng epiko ay tungkol sa paglalakbay ng binatang si Manimimbin sa paghahanap ng pag-ibig at pakikipagkaibigan at tunggalian sa mga tauhan."
            },
            new Medals
            {
                MedalId = 10,
                MedalName = "Mandirigmang Mindanao Badge",
                MedalImagePath = "medals/badgemanimimbin.png",
                isUnlocked = false,
                MedalDescription = "Makapasa sa comprehension quiz o mini-game pagkatapos ng kwento"
            },
            new Medals
            {
                MedalId = 11,
                MedalName = "Badge ng Mabuting Aral",
                MedalImagePath = "medals/badgeangaralkayloro",
                isUnlocked = false,
                MedalDescription = "Piliin ang tamang aral mula sa kwento.\nItinatampok ang kahalagahan ng pagkilala at pagsasabuhay ng tamang aral."
            },
            new Medals
            {
                MedalId = 12,
                MedalName = "Badge ng Pagkakaibigan",
                MedalImagePath = "medals/badgekabayoatkalabaw.png",
                isUnlocked = false,
                MedalDescription = "Makumpleto ang story interaction o mini-activity.\nNakatuon sa pagtutulungan, pag-unawa, at pagkakaibigan sa kwento\r\n"
            },
            new Medals
            {
                MedalId = 13,
                MedalName = "Badge ng Matiyagang Mananakbo",
                MedalImagePath = "medals/badgepagongatkuneho.png",
                isUnlocked = false,
                MedalDescription = "Makumpleto ang kwento at makapasa sa mga tanong.\nIpinapakita ang pagtitiyaga at determinasyon ng Pagong na manalo sa karera\r\n"
            },
            new Medals
            {
                MedalId = 14,
                MedalName = "Badge ng Matalinong Pagong",
                MedalImagePath = "medals/badgepagongatmatsing.png",
                isUnlocked = false,
                MedalDescription = "Matapos basahin ang kwento at masagot nang tama ang quiz.\nPinapakita ng kwento ang talino at pagiging mapanlikha ng Pagong sa pagharap sa problema\r\n"
            },
            new Medals
            {
                MedalId = 15,
                MedalName = "Badge ng Katatagan",
                MedalImagePath = "medals/badgetularansikawayan.png",
                isUnlocked = false,
                MedalDescription = "Makuha ang buong score sa quiz ng kwento\nIpinapakita ang tibay, katatagan, at pagtitiis tulad ng kawayan sa mga pagsubok\r\n"
            },
            new Medals
            {
                MedalId = 16,
                MedalName = "Badge ng Tagapangalaga ng Aral",
                MedalImagePath = "medals/specialbadgepabula.png",
                isUnlocked = false,
                MedalDescription = "Makukuha ito kapag nabasa mo ang lahat ng pabula at nasagot nang tama ang mga tanong o mini-activity tungkol sa aral ng bawat kwento.\nIpinapakita nito na naunawaan mo at nasasabuhay ang mga aral o moral na itinuturo ng mga pabula.\r\n"
            },
            new Medals
            {
                MedalId = 17,
                MedalName = "Badge ng Maliwanag na Ulo",
                MedalImagePath = "medals/badgebugtongmahusaynatagahula.png",
                isUnlocked = false,
                MedalDescription = "Kapag nasagot ng tama ang unang 10 bugtong.\nPinapakita ang talino at pagka-alerto sa palaisipan\r\n"
            },
            new Medals
            {
                MedalId = 18,
                MedalName = "Badge ng Matalinong Bata",
                MedalImagePath = "medals/badgebugtongmatalinongbata.png",
                isUnlocked = false,
                MedalDescription = "Kapag nasagot ng tama ang 13 bugtong mula 15.\nPinapakita ang kakayahang maunawaan ang mas mahihirap na bugtong\r\n"
            },
            new Medals
            {
                MedalId = 19,
                MedalName = "Badge ng Pinakamahusay na Tagahula",
                MedalImagePath = "medals/badgetamalahatngbugtong.png",
                isUnlocked = false,
                MedalDescription = "Kapag nasagot ng tama lahat ng 15 bugtong.\nPinapakita ang galing sa paghula at pagka-master ng bugtong \r\n"
            },
            new Medals
            {
                MedalId = 20,
                MedalName = "Badge ng Maliwanag na Kamay",
                MedalImagePath = "medals/badgengmaliwanagnakamay.png",
                isUnlocked = false,
                MedalDescription = "Kapag nakumpleto ang unang 5 coloring activity.\nPinapakita ang kasanayan sa paggamit ng kulay\r\n"
            },
            new Medals
            {
                MedalId = 21,
                MedalName = "Badge ng Masiglang Kulay",
                MedalImagePath = "medals/badgemasiglangmagkulay.png",
                isUnlocked = false,
                MedalDescription = "Kapag nakumpleto ang 10 coloring activity.\nPinapakita ang sipag at tiyaga sa paggawa ng makukulay na larawan\r\n"
            },
            new Medals
            {
                MedalId = 22,
                MedalName = "Badge ng Pinakamakulay na Bata",
                MedalImagePath = "medals/badgepinakamakulaynabata.png",
                isUnlocked = false,
                MedalDescription = "Kapag nakumpleto lahat ng 15 coloring activity.\nPinapakita ang galing at pagiging malikhain sa paggamit ng kulay\r\n"
            },
            new Medals
            {
                MedalId = 23,
                MedalName = "Badge ng Pagpapakain sa Narrator",
                MedalImagePath = "medals/badgeparasaunangpagbusogngnarrator.png",
                isUnlocked = false,
                MedalDescription = "Kapag unang pagkakataon na pakainin ang narrator.\nPinapakita ang pagmamalasakit at pagtulong sa narrator ng app\r\n"
            },

        };
    }
}
