using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilipinoFolkloreApp.Services
{
    public static class AlamatContent
    {
        public class Narrator
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Avatar { get; set; } = "";
            public int PriceStars { get; set; } = 0;
            public string NarratorBackground { get; set; } = "";
        }

        public class Slide
        {
            public string Background { get; set; } = "";
            public List<string> Characters { get; set; } = new();
            public string? Subtitle { get; set; }

            public Dictionary<string, string> AudioByNarrator { get; set; } = new();
        }

        public class QuizQuestion
        {
            public string Prompt { get; set; } = "";
            public List<string> ChoiceImages { get; set; } = new();
            public int CorrectIndex { get; set; } = 0;
            public int TimeLimitSec { get; set; } = 1000;
        }

        public class Story
        {
            public int StoryIndex { get; set; } = 0;
            public string Id { get; set; } = ""; // monitor
            public string Title { get; set; } = "";
            public int MedalId { get; set; } = 0;
            public int PriceStars { get; set; } = 0;
            public string Category { get; set; } = "";
            public int RewardStars { get; set; } = 20;
            public string Thumb = "";
            public bool IsRewardClaimed = false; // monitor
            public bool IsPurchased = false; // monitor
            public bool NarratorEagleUnlocked = false; // monitor
            public bool NarratorMonkeyUnlocked = false; // monitor
            public List<Slide> Slides { get; set; } = new();
            public List<QuizQuestion> Quiz { get; set; } = new();
        }

        // ------- EDIT THESE: narrators, stories, slides, quiz -------
        public static List<Narrator> Narrators { get; } = new()
        {
            new Narrator{ Id="tarsier", Name="Tarsier", Avatar="elements/tarsier.png",NarratorBackground ="Ako si Tarsy maliit lang ako, pero may malalaking mata na nakakatulong sa akin na makita kahit madilim ang paligid. Mula pagkabata, ako na ang bantay ng gubat tuwing gabi. Ako ang nagbabantay para siguraduhing ligtas ang lahat. Dito sa laro, tutulungan kitang makita ang mga bagay na mahirap makita kapag gabi at turuan kang maging matapang kahit madilim ang paligid. Pinili ako dahil gusto kong ipakita na kahit maliit ka, may malaking bagay kang magagawa kapag matapang ka at mapagmasid sa iyong paligid. Tara, sasamahan kita na maglakbay sa gabi.", PriceStars=0 },
            new Narrator{ Id="eagle",   Name="Agila",   Avatar="elements/eagle.png",NarratorBackground="Ako si Agie isang agila na malakas ang pakpak at mabilis lumipad sa himpapawid. Nakikita ko ang buong mundo mula sa taas ng langit. Palagi akong nagmamasid kung ano ang nangyayari mula sa itaas, para maprotektahan ang mga naninirahan sa gubat sa baba. Pinili mo ako para maging iyong gabay dahil gusto kitang turuan na maging matapang at laging handang tumulong sa oras ng problema. Dito sa laro, tutulungan kitang tingnan ang mga bagay mula sa malayo at mapag matyag, para lagi kang handa sa kahit anong mangyayari. Sama na, lipad tayo at alamin ang mga sikreto ng gubat",   PriceStars=50 },
            new Narrator{ Id="monkey",  Name="Unggoy",  Avatar="elements/monkey.png",NarratorBackground ="Ako si Makkie ang unggoy na laging masaya at malikot. Mahilig ako tumalon-talon sa mga puno at maghanap ng masasarap na saging na paborito kong prutas. Palagi akong naglalaro sa gubat at nakikisama sa iba pang mga hayop. Pinili mo ako para maging narrator dahil gusto kong ipakita sa'yo na ang gubat ay puno ng saya at magkaron ng mga kaibigan. Dito sa laro, tutulungan kitang maghanap ng mga kayamanan at matuto habang ikaw ay naglalaro. Halika samahan mo ako sa masayang pakikipagsapalaran sa gubat.",  PriceStars=100 },
        };

        public static List<Story> Stories { get; } = new()
        {
            //Alamat
            //new Story{
            //    StoryIndex=1, Category = "alamat",
            //    Id="1_juan_tamad", Title="Juan Tamad", PriceStars=0, RewardStars=20, Thumb ="stories/juantamad/juan_tamad_thumb.png",
            //    Slides = new()
            //    {
            //        new Slide{
            //            Background="stories/juantamad/juan_tamad_scene1.png",
            //            Characters=new(){ "stories/juan/char_boy.png" },
            //            Subtitle="Si Juan ay naghihintay sa ilalim ng puno.",
            //            AudioByNarrator = new()
            //                    {
            //                        ["tarsier"] = "storiesalamat/juantamad/audio/juantamad_tarsier1.mp3",
            //                        ["eagle"]   = "storiesalamat/juantamad/audio/juantamad_tarsier1.mp3",
            //                        ["monkey"]  = "storiesalamat/juantamad/audio/juantamad_tarsier1.mp3",
            //                    }
            //            },
            //        new Slide{
            //            Background="stories/juantamad/juan_tamad_scene2.png",
            //            Characters=new(){ "stories/juan/char_boy.png","stories/juan/char_tarsier.png" },
            //            Subtitle="Inaabangan niya ang pagbagsak ng bunga.",
            //            AudioByNarrator = new()
            //                    {
            //                        ["tarsier"] = "storiesalamat/juantamad/audio/juantamad_tarsier2.mp3",
            //                        ["eagle"]   = "storiesalamat/juantamad/audio/juantamad_tarsier2.mp3",
            //                        ["monkey"]  = "storiesalamat/juantamad/audio/juantamad_tarsier2.mp3",
            //                    }
            //            },
            //    },
            //    Quiz = new()
            //    {
            //        new QuizQuestion{
            //            Prompt="Sino ang pangunahing tauhan?",
            //            ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
            //            CorrectIndex=0, TimeLimitSec=20
            //        }
            //    }
            //},
            new Story{
                StoryIndex=1, Category = "alamat", MedalId = 1,
                Id="1_juan_tamad", Title="Juan Tamad", PriceStars=0, RewardStars=20, Thumb ="stories/juantamad/juan_tamad_thumb.png",
                Slides = new()
                {
                    new Slide{
                        Background="stories/juantamad/juan_tamad1.PNG",
                        Subtitle="Isang araw, sa isang maliit na baryo, naroroon si Juan Tamad na may matinding nais na kumain ng malamig na bunga ng bayabas."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad2.PNG",
                        Subtitle="Sa halip na pitasin ito mula sa puno, pinili niyang maghintay na lamang sa ilalim ng puno at abangan ang pagbagsak ng bunga. Habang siya’y nakaupo’t nag-aabang,"
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad3.PNG",
                        Subtitle="biglang dumating si Mariang Masipag, isang dalagang masipag at masinop."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad4.PNG",
                        Subtitle="Agad nitong pinitas ang bungang inaasam ni Juan Tamad."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad5.PNG",
                        Subtitle="Nang makita si Juan na bigo sa kanyang planong kumain, nagtampo siya kay Mariang Masipag."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad6.PNG",
                        Subtitle="“Naku, Juan Tamad! Hindi ka talaga matututo,” sabi ni Mariang Masipag. “Dapat ay gawin natin ang mga bagay na nararapat at huwag tayong maghintay na dumating ang lahat sa atin nang walang kahit anong pagsisikap.”"
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad7.PNG",
                        Subtitle="Sa kanyang pag-iling-iling, na-amaze si Juan Tamad sa katalinuhan at kasipagan ni Mariang Masipag."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad8.PNG",
                        Subtitle="Kinabukasan, naisipan niyang tuparin ang pangarap na ligawan si Mariang Masipag. Lumapit siya sa bahay ng dalaga,"
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad9.PNG",
                        Subtitle="ngunit doon niya natagpuan ang ina ni Mariang Masipag na tila ba hindi siya gusto para sa kanyang anak."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad10.PNG",
                        Subtitle="Tinanong siya ng ina kung bakit siya naroroon, at doon nagsimula ang usapan. Nang itanong kung bakit “Juan Tamad” ang pangalang kaniyang dala, sinimulan niyang ibahagi ang dalawang pangyayaring naging dahilan ng pangalang iyon."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad11.PNG",
                        Subtitle="Una, nagkwento si Juan Tamad kung paano siya naging tamad sa pagtinda ng puto ng kanyang ina. Sa sobrang init ng panahon, nagpasya siyang magpahinga na lamang kaysa tignan ang kalidad ng kanyang tinitindang produkto. Inabot siya ng antok at hindi napansin na kinain na pala ng mga palakang gutom ang lahat ng kanyang puto. Nang makarating siya sa bahay, kinailangan niyang magtago ng dalawang palakeng gutom upang hindi siya mapagalitan ng kanyang ina."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad12.PNG",
                        Subtitle="Ikalawa, inihayag ni Juan Tamad ang kwento tungkol sa pagtanggap ng kanyang ina ng trabaho na magbenta ng palayok sa palengke. Dahil sa kagustuhang kumita ng pera, nagdesisyon siyang maglakad papunta sa palengke nang may dalang palayok. Sa daan, nakasalubong niya si Mariang Masipag na nagmamaneho ng bisikleta."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad12.PNG",
                        Subtitle="Dahil sa antok at kaantukan, nabangga ni Juan si Mariang Masipag, at nasira ang mga bitbit nitong palayok. Dahil doon, napilitan siyang gumawa ng paraan para magkaroon ng pambayad: binayaran niya ang mga palayok nang pino-pino, itinaga sa malambot na dahon, at ipinakalat bilang “gamot sa galis.”"
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad13.PNG",
                        Subtitle="Ngunit sa kabila ng pagbabago at pagsusumikap na ito, nagpatuloy pa rin ang pagiging tamad ni Juan. Sinubukan niyang manligaw kay Mariang Masipag, ngunit inutos ng ina nito na umuwi na at huwag nang bumalik. Sa halip na panghinaan ng loob, tinanggap ni Juan ang hamon na ipinahayag ng mga magulang ni Mariang Masipag."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad14.PNG",
                        Subtitle="Kailangang patunayan ni Juan ang kanyang sarili, at dala ng determinasyon, nagpasya siyang baguhin ang kanyang mga gawi."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad15.PNG",
                        Subtitle="Nag-aral siyang magsagawa ng mga gawaing bahay, nagtrabaho nang masipag, at naging masinop sa mga bagay. Habang sumusulong ang panahon, hindi na kinikilala si Juan bilang “Tamad” kundi bilang “Tama” dahil sa mga positibong pagbabago sa kanyang buhay."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad16.PNG",
                        Subtitle="Matapos ng mga pagsubok, inabot din niya ang kanyang pangarap na mapalapit kay Mariang Masipag. Nang magkatagpo sila ulit, nagpahayag si Juan ng pagmamahal at pag-aalaga. Natutunan niya ang kahalagahan ng sipag, disiplina, at pagsusumikap, at nakuha niya ang pagkilala at respeto hindi lamang ng kanyang ina kundi maging ng mga magulang ni Mariang Masipag."
                    },

                },
                Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
            },
            new Story{
                StoryIndex =2,Category = "alamat", MedalId = 2,
                Id="2_chocolate_hills", Title="Alamat ng Chocolate Hills", PriceStars=1, RewardStars=20,Thumb="stories/chocolatehills/chocolate_hillsicon.png",
                Slides=new()
                {
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills1.png",
                        Subtitle="Noong unang panahon, sa probinsiya ng Bohol, parting Kabisayaaan, may lupang malawak \r\nsubali’t ito ay tuyot. Makikita mong biyak-biyak ang lupain kapag tag-init. Talagang \r\npagpapawisan ka kapag napadaan ka sa lugar. Subali’t kapag tag-ulan ito ay maputik at \r\nsiguradong mababaon ang iyon paa kapag ikaw ay naka-yapak. Ngunit kung araw ng taniman \r\nay maaliwalas ang kapaligiran sa kulay ng berdeng tanawin ng pook. "
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills2.png",
                        Subtitle="Ayon sa matatanda roon, may isang araw sa magkabilang dulo ng isla na may dalawang \r\nhiganteng dumating. Ang isa ay nagmula sa parting timog at ang isa naman ay sa hilaga. Ang \r\nmga naninirahan doon ay nangangamba na baka magkita ang dalawa. Kaya’t nilisan \r\npansamantala ng tagaroon ang lugar. Sa inaasahang pangyayari nagkita nga ang dalawang \r\nhigante. \r\n"
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills3.png",
                        Subtitle="Higanteng naka brown: \r\n“Anong ginagawa mo sa aking nasasakupan!” Ito’y aking pag-aari at umalis ka na,” galit na \r\nsinabi ni Higanteng mula saTimog . ” Maghanap ka ng lugar na iyong aangkinin.”"
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills4.png",
                        Subtitle="Higanteng naka green: \r\n“Aba!, ako yata ang nauna rito at ito’y pag-aari ko na!” sagot ding galit ng higante mula sa \r\nhilaga. “Ikaw dapat ang umalis!” \r\n"
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills5.png",
                        Subtitle="Higanteng naka brown: \r\n“Hindi maaari ito! Ito ay pag-aari ko!” sabay padyak ng Higante mula sa Timog at nayanig ang \r\nlugar na parang lumilindol. \r\n"
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills6.png",
                        Subtitle="Higanteng naka green: \r\n“Lalong hindi maaari!” mas malakas ang padyak ng Higante mula sa Hilaga."
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills7.png",
                        Subtitle="Noong panahong iyon, ay katatapos pa lamang ang tag-ulan at maputik sa kinatatayuan nila. \r\nGinawa ng isang higante ay bumilog ng putik at binato sa isa."
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills8.png",
                        Subtitle="Subali’t gumanti rin ang isa at humulma rin ng isang bilog na putik at siya ring binato sa \r\nkalaban. \r\n"
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills9.png",
                        Subtitle="Walang tigil na batuhan ng binilog na putik."
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills10.png",
                        Subtitle="Hanggang ang dalawa ay hingalin, naubusan ng lakas at nawalan ng hininga."
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills11.png",
                        Subtitle="Tumumba ang dalawang higante na wala ng buhay.Marami ang nakasaksi sa pangyayari na \r\ntagaroon."
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills12.png",
                        Subtitle="Ang sumabat sa paningin ng mga tao ang mala-higanteng bolang putik na siyang ginamit ng \r\nmga naabing higante sa pagbabatuhan. \r\n"
                    },
                    new Slide{
                        Background="stories/chocolatehills/chocolate_hills13.png",
                        Subtitle="Pagkatapos ng pangyayari, nagsibalikan ang naninirahan doon. Namuhay ng mapayapa at \r\nmasagana.Dahil sa bulubunduking ginawa ng mga higante na kulay  tsokolate na sila ring \r\nnapakikinabangang taniman, ito ang pinagmulan ng Chocolate Hills."
                    },
                },
                Quiz=new(){ new QuizQuestion{ Prompt="Ano ang paksa?",
                           ChoiceImages=new(){ "quiz/choco/a.png","quiz/choco/b.png","quiz/choco/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                StoryIndex =3,Category = "alamat", MedalId = 3,
                Id="3_langit", Title="Langit", PriceStars=2, RewardStars=50,
                Thumb= "stories/langit/LANGIT1.png",
                Slides=new()
                {
                    new Slide{
                        Background="stories/langit/langit1.png",
                        Subtitle="Bakit May Araw, Buwan, at mga Bituin?\r\nNoon, maaaring maabot ang langit. Gumawa si Bathala ng isang lalaki at isang babae. Ang kasipagan nila ay walang katulad. Nagtatanim sila at nagbabayo ng palay.\r\n"
                    },
                    new Slide{
                        Background="stories/langit/langit2.png",
                        Subtitle="Minsan, umalis ang lalaki para magtanim. Mangunguha rin siya ng prutas. Nagbayo naman ng palay ang babae. Nakasasagabal sa kaniyang pagbabayo ang kaniyang brilyanteng kuwintas at suklay. Tinanggal niya ang mga ito at isinabit sa ulap."
                    },
                    new Slide{
                        Background="stories/langit/langit3.png",
                        Subtitle="Nakapagbayo na siya nang kaunti at sapat sa pananghalian. Kumuha siya ng panggatong at nagsaing sa tabi ng mababang ulap. Habang nagsasaing ay muli siyang nagbayo para sa kanilang hapunan."
                    },
                    new Slide{
                        Background="stories/langit/langit4.png",
                        Subtitle="Maya-maya ay dumating na ang lalaki. Bitbit niya ang ilang prutas. Naabutan niyang nagbabayo ang babae. Tinulungan niya ang babae sa pagbabayo. Hirap siyang iunat ang kaniyang mga braso. Ginamit ng lalaki ang malaki niyang pambayo ng palay. Tinatamaan ang langit tuwing babayo ang lalaki. Kaya siya ay humiling sa Bathala, \"Bathala, pataasin mo po ang langit”"
                    },
                    new Slide{
                        Background="stories/langit/langit5.png",
                        Subtitle="Biglang lumakas ang hangin at unti-unti nang tumaas ang langit. Muling tinamaan ang langit ng pambayo ng lalaki. Muling pinataas ng Bathala ang langit. Kasabay nito, tumaas din ang mga ulap. Tumaas din ang brilyanteng kuwintas at suklay. Tumaas din ang palayok a apoy sa tabi ng mga ulap.\r\nAng pagtaas ng langit ay hindi na mapigilan.\r\n "
                    },
                    new Slide{
                        Background="stories/langit/langit6.png",
                        Subtitle="Ang pagtaas ng langit ay hindi na mapigilan. "
                    },
                    new Slide{
                        Background="stories/langit/langit7.png",
                        Subtitle="Hindi nagtagal, ang langit ay tuluyan nang tumaas at hindi na maabot. Hindi na natanggal sa pagkakasabit ang mga kuwintas at suklay. Mula noon, nagkaroon na ng suklay sa langit na hugis kabiyak na buwan. Nagkaroon din ng makikinang na parang mga brilyante na naging bituin. Ang palayok na may kanin ay naging maliwanag na buwan. Ang apoy naman ay naging maliwanag na araw tuwing umaga."
                    },

                }, // no subtitle example
               
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },
            new Story{
                StoryIndex=4,Category = "alamat", MedalId= 4,
                Id="4_mangga", Title="Alamat ng Mangga", PriceStars=3, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/mangga/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong prutas?",
                           ChoiceImages=new(){ "quiz/mangga/a.png","quiz/mangga/b.png","quiz/mangga/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                StoryIndex= 5,Category = "alamat", MedalId = 5,
                Id="5_saging", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },
            
            //Epiko
            new Story{
                StoryIndex =21,Category = "epiko", MedalId =6,
                Id="6_Lapulapu", Title="Lapu-Lapu", PriceStars=50, RewardStars=25,Thumb ="storiesepiko/lapulapu/lapulapu_thumbnail.png",
                Slides = new()
                {
                    new Slide{
                        Background="storiesepiko/lapulapu/lapulapu_scene1.png",
                        Characters=new(){ "stories/juan/char_boy.png" },
                        Subtitle="Si Lapu-Lapu ay isang napakakisig na bayani.",
                        AudioByNarrator = new()
                                {
                                    ["tarsier"] = "storiesepiko/lapulapu/audio/lapulapu_tarsier1.mp3",
                                    ["eagle"]   = "storiesepiko/lapulapu/audio/lapulapu_tarsier1.mp3",
                                    ["monkey"]  = "storiesepiko/lapulapu/audio/lapulapu_tarsier1.mp3",
                                }
                        },
                    new Slide{
                        Background="storiesepiko/lapulapu/lapulapu_scene2.png",
                        Characters=new(){ "stories/juan/char_boy.png","stories/juan/char_tarsier.png" },
                        Subtitle="Siya ang unang lumaban sa mga kastila.",
                        AudioByNarrator = new()
                                {
                                    ["tarsier"] = "storiesepiko/lapulapu/audio/lapulapu_tarsier2.mp3",
                                    ["eagle"]   = "storiesepiko/lapulapu/audio/lapulapu_tarsier2.mp3",
                                    ["monkey"]  = "storiesepiko/lapulapu/audio/lapulapu_tarsier2.mp3",
                                }
                        },
                },
                Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    },
                    new QuizQuestion{
                        Prompt="Sino ang kontrabida sa kwento?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
            },
            new Story{
                StoryIndex = 22,
                Category = "epiko",
                Id = "6_bantugan",
                Title = "Bantugan",
                MedalId = 7,
                PriceStars = 0,
                RewardStars = 50,
                Thumb = "storiesepiko/bantugan/bantugan1.png",
                Slides = new()
                {
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan1.png",
                        Subtitle = "Si Bantugan ay isang magiting na mandirigma sa epikong-bayang Darangan ng mga Maranaw. Siya ay kilala sa kaniyang kahariang Bumbaran dahil sa mga naipanalo niyang mga digma at labanan."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan2.png",
                        Subtitle = "Sagisag ng tapang at kakisigan, si Prinsipe Bantugan ay sikat na sikat sa kanilang kahariang Bumbaran lalo na sa mga dalaga."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan3.png",
                        Subtitle = "Sinasabing naligawan na niya ang 50 na pinakamagandang prinsesa sa mundo."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan4.png",
                        Subtitle = "Dahil dito, lubhang naiinggit sa kaniya ang mas nakatatandang kapatid na si Haring Madali. Ipinagbawal ni Madali na kausapin ng kahit sino ang kaniyang kapatid."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan5.png",
                        Subtitle = "Sa labis na kalungkutan, umalis ng kanilang kaharian si Bantugën."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan6.png",
                        Subtitle = "Hanggang nagkasakit at namatay malapit sa Kaharian ng Lupaing nása Pagitan ng Dalawang Dagat."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan7.png",
                        Subtitle = "Nakita ng hari at ni Prinsesa Datimbang ang katawan ni Bantugan."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan8.png",
                        Subtitle = "Agad nilang inilapit ang balita sa pulong ng mga tagapayo."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan9.png",
                        Subtitle = "Isang loro ang pumasok at sinabi kung sino at kung saan gáling ang patay na manlalakbay."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan10.png",
                        Subtitle = "Nang mabalitaan ito ni Haring Madali, binawi niya ang kaluluwa ng kapatid sa langit upang maibalik sa katawan ni Bantugën."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan11.png",
                        Subtitle = "Kumalat ang balita ng kaniyang pagkabuhay hanggang sa kaaway na kaharian at kay Haring Miskoyaw."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan12.png",
                        Subtitle = "Sinugod ng kawal ni Miskoyaw ang Bumbaran at nabihag si Prinsipe Bantugën na may nanghihinà pang katawan."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan13.png",
                        Subtitle = "Nang magbalik ang lakas, pinuksa niya ang hukbo ng kaaway na hari at iniligtas ang buong Bumbaran."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan14.png",
                        Subtitle = "Nagkaroon silá ng malaking pagdiriwang at nawala na ang inggit sa puso ni Haring Madali."
                    },
                    new Slide{
                        Background = "storiesepiko/bantugan/bantugan15.png",
                        Subtitle = "Matagal at masayang namuhay sa kaharian ng Bumbaran si Prinsipe Bantugan kasáma ng mga pinakasalan niyang prinsesa."
                    }
                },
                Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    },
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
            },
            new Story{
            StoryIndex = 23,
            Category = "epiko",
            Id = "6_ibalon",
            Title = "Ibalon",
            MedalId = 8,
            PriceStars = 50,
            RewardStars = 50,
            Thumb = "storiesepiko/ibalon/ibalon1.png",
            Slides = new()
            {
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon1.png",
                    Subtitle = "Si Baltog ay nakarating sa lupain ng Ibalon dahil sa pagtugis niya sa isang malaking baboy-ramo. Siya’y nanggaling pa sa lupain ng Batawara. Mayaman ang lupain ng Ibalon at doon na siya nanirahan. Siya ang kinilalang hari ng Ibalon. Naging maunlad ang pamumuhay ng mga tao."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon2.png",
                    Subtitle = "Subalit may muling kinatakutan ang mga tao, isang malaki at mapaminsalang baboy-ramo na tuwing sumasapit ang gabi ay namiminsala ng mga pananim."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon3.png",
                    Subtitle = "Si Baltog ay matanda na upang makilaban. Tinulungan siya ng kanyang kaibigang si Handiong."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon4.png",
                    Subtitle = "Pinamunuan ni Handiong ang mga lalaki ng Ibalon upang kanilang lipulin ang mga dambuhalang buwaya, mababangis na tamaraw at lumilipad na mga pating at mga halimaw na kumakain ng tao.\n Napatay nila ang mga ito maliban sa isang engkantadang nakapag-aanyong magandang dalaga na may matamis na tinig."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon5.png",
                    Subtitle = "Ito ay si Oriol. Tumulong si Oriol sa paglipol ng iba pang mga masasamang hayop sa Ibalon."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon6.png",
                    Subtitle = "Naging payapa ang Ibalon. Ang mga tao ay umunlad. Tinuruan niya ang mga tao ng maayos na pagsasaka. Ang mga piling tauhan ni Handiong ay tumulong sa kanyang pamamahala at pagtuturo sa mga tao ng maraming bagay."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon7.png",
                    Subtitle = "Ang sistema ng pagsulat ay itinuro ni Sural. Itinuro ni Dinahong Pandak ang paggawa ng palayok na Iluad at ng iba pang kagamitan sa pagluluto."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon8.png",
                    Subtitle = "Si Hablon naman ay nagturo sa mga tao ng paghabi ng tela. Si Ginantong ay gumawa ng kauna-unahang bangka, ng araro, itak at iba pang kasangkapan sa bahay."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon9.png",
                    Subtitle = "Naging lalong maunlad at masagana ang Ibalon. Subalit may isang halimaw na namang sumipot. Ito ay kalahating tao at kalahating hayop na si Rabut."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon10.png",
                    Subtitle = "Nagagawa niyang bato ang mga tao o hayop na kanyang maengkanto. Nabalitaan ito ni Bantong at inihandog niya ang sarili kay Handiong upang siyang pumatay kay Rabut."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon11.png",
                    Subtitle = "Nalaman ni Bantong na sa araw ay tulog na tulog si Rabut. Kaniya itong pinatay habang natutulog."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon12.png",
                    Subtitle = "Nagalit ang Diyos sa ginawang pataksil na pagpatay kay Rabut. Diumano, masama man si Rabut, dapat ay binigyan ng pagkakataong magtanggol sa sarili nito."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon13.png",
                    Subtitle = "Pinarusahan ng Diyos ang Ibalon sa pamamagitan ng isang napakalaking baha."
                },
                new Slide{
                    Background = "storiesepiko/ibalon/ibalon14.png",
                    Subtitle = "Nasira ang mga bahay at pananim. Nalunod ang maraming tao. Nakaligtas lamang ang ilang nakaakyat sa taluktok ng matataas na bundok. Nang kumati ang tubig, iba na ang anyo ng Ibalon. Nagpanibagong buhay ang mga tao ngayon ay sa pamumuno ni Bantong."
                }
            },
            Quiz=new(){ new QuizQuestion{ Prompt="Ano ang paksa?",
                           ChoiceImages=new(){ "quiz/choco/a.png","quiz/choco/b.png","quiz/choco/c.png"},
                           CorrectIndex=1 } }
        },
            new Story{
            StoryIndex = 24,
            Category = "epiko",
            Id = "6_kudaman",
            Title = "Kudaman",
            MedalId = 9,
            PriceStars = 0,
            RewardStars = 50,
            Thumb = "storiesepiko/kudaman/kudaman1.png",
            Slides = new()
            {
                new Slide{
                    Background = "storiesepiko/kudaman/kudaman1.png",
                    Subtitle = "Nagsisimula ito sa istorya kung paano napangasawa ni Kudaman si Tuwan Putli, at pagkaraan, ang tatlo pang asawa na nagturingang magkakapatid at nagsáma-sáma sa isang tahanan."
                },
                new Slide{
                    Background = "storiesepiko/kudaman/kudaman2.png",
                    Subtitle = "Sinundan ito ng pagdalo sa isang pagdiriwang ng mga Ilanun upang manggulo. Ilang taóng naglaban si Kudaman at ang pinunòng Ilanun at sa ganitong labanan ay nagwawagi sa dulo ang bayani upang kaibiganin ang nakalaban."
                },
                new Slide{
                    Background = "storiesepiko/kudaman/kudaman3.png",
                    Subtitle = "Anupa’t malimit magtapos ang mga bahagi ng tultul sa malaking inuman ng tabad, ang alak ng Palawan, at pagkonsumo ng mahigit sandaang tapayan ng alak."
                },
                new Slide{
                    Background = "storiesepiko/kudaman/kudaman4.png",
                    Subtitle = "Dili kayâ’y nagsisimula ito sa malaking inuman na nauuwi sa labanan kapag nalasing ang mga panauhin. Sa dulo ng mga nairekord na tultul, sampu na ang asawa ni Kudaman na nakatagpo sa iba’t ibang abentura."
                },
                new Slide{
                    Background = "storiesepiko/kudaman/kudaman5.png",
                    Subtitle = "Gayunman, mapapansin diumano ang taglay na hinahon at paghahangad ng kapayapaan ni Kudaman. Maraming tagpo ng sigalot na tinatapos sa kasunduang pangkapayapaan at pagpapasiya alinsunod sa tradisyong Palawan."
                },
                new Slide{
                    Background = "storiesepiko/kudaman/kudaman6.png",
                    Subtitle = "Nakapalaman din sa tultul ang mga kapaniwalaan ng Palawan at ang konsepto nilá ng sandaigdigan."
                }
            },
            Quiz=new(){ new QuizQuestion{ Prompt="Ano ang paksa?",
                           ChoiceImages=new(){ "quiz/choco/a.png","quiz/choco/b.png","quiz/choco/c.png"},
                           CorrectIndex=1 } }
        },
        new Story{
                StoryIndex =25,Category = "epiko", MedalId = 10,
                Id="3_maton", Title="Alamat ng Maton", PriceStars=2, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/maton/s1.png", Subtitle="..." } }, // no subtitle example
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },

            new Story{
                StoryIndex = 31,Category = "pabula", MedalId = 11,
                Id="6_pagong_at_kuneho", Title="Pagong at si Kuneho", PriceStars=0, RewardStars=25,Thumb ="storiespablula/pagongatkuneho/pagong_at_kuneho1.png",
                Slides = new()
                {
                    new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho1.png",
                    Subtitle="Isang araw habang naglalakad si Kuneho ay nakasalubong niya si Pagong. Palibhasa makupad maglakad ang pagong kaya pinagtawanan ito ng kuneho at nilibak."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho2.png",
                    Subtitle="“Napakaiksi ng mga paa mo Pagong, kaya ubod ka ng bagal maglakad, wala kang mararating niyan.” At sinundan iyon ng malulutong na tawa."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho3.png",
                    Subtitle="Labis na nainsulto ang Pagong sa mga sinabi ng Kuneho. Para patunayan na nagkakamali ito ng akala ay hinamon niya ang Kuneho."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho4.png",
                    Subtitle="“Maaaring mabagal nga akong maglakad, subalit matibay ang katawan ko, hindi mo ako matatalo.” Lalo lamang siyang pinagtawanan."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho5.png",
                    Subtitle="“Nabibigla ka yata Pagong, baka mapahiya ka lamang,” wika ni Kuneho. “Para magkasubukan tayo, magkarera tayo patungo sa ituktok ng bulubunduling iyon.” Itinuro ni Pagong ang abot-tanaw na bundok."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho6.png",
                    Subtitle="Ganoon na lamang ang katuwaan ng mayabang na Kuneho sa hamon na iyon ni Pagong. Nagtawag pa ito ng mga kaibigan para manood sa gagawin nilang karera. Gusto niyang lalong libakin si Pagong sa harap ng kanyang mga kaibigan oras na matalo niya ito. Nakapaligid sa kanila ang mga kaibigang hayop. Si Matsing ang nagbilang para sa pag-uumpisa ng paligsahan."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho7.png",
                    Subtitle="“Handa na ba kayo?”. Magkasabay na tumugon sina Pagong at Kuneho, “Handa na kami!”. “Isa..Dalawa..Tatlo!..takbo!”, sigaw ni Matsing."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho8.png",
                    Subtitle="Magkasabay ngang humakbang ang dalawa mula sa lugar ng pag-uumpisahan. Mabilis na nagpalundag-lundag si Kuneho."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho9.png",
                    Subtitle="Halos sandaling minuto lamang ay naroroon na siya sa paanan ng bundok. Nang lumingon siya ay nakita niyang malayung-malayo ang agwat niya kay Pagong."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho10.png",
                    Subtitle="Patuloy sa kanyang mabagal na paglakad si Pagong, habang pinagtatawanan siya ng mga nakapaligid na hayop. Hindi pansin ni Pagong ang panunuya ng mga ito. Patuloy siya sa paglakad, walang lingun-lingon."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho11.png",
                    Subtitle="Samantala, si Kuneho ay halos mainip na sa paghihintay na makita si Pagong sa kanyang likuran. Ilang ulit na ba siyang nagpahinto-hinto, pero wala ni anino ni Pagong."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho12.png",
                    Subtitle="Palibhasa malaki ang tiwala niya sa sarili, alam niya ang kakayahan niyang tumakbo nang mabilis, ipinasya niyang maidlip muna nang makarating na siya sa kalagitnaan ng bundok. Tutal nakatitiyak naman siya ng panalo."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho13.png",
                    Subtitle="Patuloy naman sa kanyang mabagal na paglakad si Pagong paakyat, hanggang sa marating niya ang kalagitnaan ng bundok,"
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho14.png",
                    Subtitle="Naraanan pa niya si Kuneho na mahimbing na natutulog at malakas na naghihilik."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho15.png",
                    Subtitle="Nilampasan niya ito at nagpatuloy siya sa paglakad hanggang sa marating niya ang hangganan ng kanilang karera."
                },
                new Slide{
                    Background="storiespabula/pagongatkuneho/pagong_at_kuneho16.png",
                    Subtitle="Nang magising naman si Kuneho ay muli itong tumingin sa ibaba ng bundok, subalit hindi pa rin makita si Pagong. Humanda na siyang maglakad muli paakyat ng bundok, subalit ganoon na lamang ang gulat niya nang matanaw si Pagong na naroroon na sa ituktok ng bundok. Naunahan na pala siya."
                },

                },
                Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
            },
            new Story{
            StoryIndex = 32,
            Category = "pabula",
            Id = "6_pagong_at_buwaya",
            Title = "Pagong at Buwaya",
            MedalId =  12,
            PriceStars = 0,
            RewardStars = 50,
            Thumb = "storiespabula/pagongatbuwaya/pagong_at_buwaya1.png",
            Slides = new()
            {
                new Slide{
                    Background = "storiespabula/pagongatbuwaya/pagong_at_buwaya1.png",
                    Subtitle = "Isang araw, habang naghahanap ng pagkain ang matalinong unggoy sa tabi ng ilog, nakita niya ang puno ng makopa na hitik na hitik sa hinog na bunga. Ang puno ay nasa kabilang pampang lang ng ilog kung saan nakatira ang batang buwaya."
                },
                new Slide{
                    Background = "storiespabula/pagongatbuwaya/pagong_at_buwaya2.png",
                    Subtitle = "Matapos niyang makain ang lahat ng prutas na gusto niya, bumaba na sa puno ang unggoy at napagpasiyahang pumunta sa kabila ng malawak na ilog, ngunit hindi niya alam kung paano."
                },
                new Slide{
                    Background = "storiespabula/pagongatbuwaya/pagong_at_buwaya3.png",
                    Subtitle = "Sa wakas, nakita niya ang buwaya na kagigising lamang mula sa kaniyang siyesta."
                },
                new Slide{
                    Background = "storiespabula/pagongatbuwaya/pagong_at_buwaya4.png",
                    Subtitle = "Magiliw na nagwika ang unggoy, “Mahal kong buwaya, puwede bang humingi ng pabor?”\nNabigla ang buwaya sa ganitong kabait na pagbati ng unggoy.\nPero mapagkumababa itong sumagot, “Oo ba! Kung anuman ang maaaring maitulong ko sa iyo, malugod ko itong gagawin.”\nSinabi ng unggoy sa buwaya na gusto niyang pumunta sa kabilang dako ng ilog.\nSabi ng buwaya, “Buong puso kitang ihahatid doon. Umupo ka lang sa likod ko at aalis tayo kaagad.”"
                },
                new Slide{
                    Background = "storiespabula/pagongatbuwaya/pagong_at_buwaya5.png",
                    Subtitle = "Nang nakapirme na sa pagkakaupo ang unggoy sa likod ng buwaya, nagsimula na silang maglakbay. Hindi nagtagal, narating nila ang kalagitnaan ng ilog, at nagsimulang humalakhak ang buwaya."
                },
                new Slide{
                    Background = "storiespabula/pagongatbuwaya/pagong_at_buwaya6.png",
                    Subtitle = "“Ngayon, unggoy na uto-uto,” sabi niya, “kakainin ko ang iyong atay at mga bato dahil gutom na gutom na ako.”\nKinabahan ang unggoy pero hindi niya ipinahalata. Sa halip, sinabi niya, “Pinaghandaan ko na yan! Naisip ko nang baka nagugutom ka kaya inihanda ko na ang aking atay at mga bato para sa hapunan mo. Sa kasamaang-palad, naiwan kong nakasabit ang mga ito sa puno ng makopa dahil sa pagmamadali natin. Masaya ako na nabanggit mo iyan. Bumalik tayo at kukunin ko ang pagkain para sa iyo.”"
                },
                new Slide{
                    Background = "storiespabula/pagongatbuwaya/pagong_at_buwaya7.png",
                    Subtitle = "Sa pag-aakala ng uto-utong buwaya na nagsasabi ng totoo ang unggoy, bumalik ito sa tabing-ilog na pinanggalingan nila. Nang malapit na sila, mabilis na lumundag ang unggoy sa tuyong lupa at kumaripas ng takbo paakyat sa puno.\nNang makita ng buwaya kung paano siya nalinlang, sabi niya, “Isa akong uto-uto”."
                }
            },
            Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    },
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
        },
            new Story{
            StoryIndex = 33,
            Category = "pabula",
            Id = "6_pagong_at_matsing",
            Title = "Pagong at Matsing",
            MedalId = 13,
            PriceStars = 0,
            RewardStars = 50,
            Thumb = "storiespabula/pagongatmatsing/pagong_at_matsing1.png",
            Slides = new()
            {
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing1.png",
                    Subtitle = "Sina Pagong at Matsing ay matalik na magkaibigan. Mabait at matulungin si Pagong, subalit si Matsing ay tuso at palabiro. Isang araw sila ay binigyan ni Aling Muning ng isang supot ng pansit. “Halika Matsing, kainin natin ang pansit”, nag-aayang sabi ni Pagong.\n“Naku baka panis na yan” sabi ni Matsing.\n“Ang mabuti pa, hayaan mo muna akong kumain n’yan para masiguro natin na walang lason ang pagkain,” dagdag pa nito."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing1.png",
                    Subtitle = "“Hindi naman amoy panis Matsing at saka hindi naman magbibigay ng panis na pagkain si Aling Muning,” sabi ni Pagong.\n“Kahit na, ako muna ang kakain,” pagmamatigas ni Matsing.\nWalang nagawa ang kawawang Pagong kundi pagbigyan ang makulit na kaibigan. Naubos ni Matsing ang pansit at walang natira para kay Pagong.\n“Pasensya ka na kaibigan, napasarap ang kain ko ng pansit kaya wala ng natira. Sa susunod ka na lang kumain,” paliwanag ng tusong matsing."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing2.png",
                    Subtitle = "Dahil sa likas na mabait at pasensyoso si Pagong, hindi na siya nakipagtalo sa kaibigan."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing3.png",
                    Subtitle = "Sa kanilang paglilibot sa kagubatan, nakakita si Pagong ng isang puno ng saging.\n“Matsing! Matsing! tignan mo ang puno ng saging na ito. Maganda ang pagkakatubo. Gusto ko itong itanim sa aking bakuran para pag nagkabunga ay makakain natin ito,” masayang sabi ni Pagong.\n“Gusto ko rin ng saging na ‘yan Pagong, ibigay mo na lang sa akin,” sabi ni Matsing."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing3.png",
                    Subtitle = "“Pasensya ka na, gusto ko rin kasi nito. Kung gusto mo hatiin na lang natin.”\n“Hahatiin? O sige pero sa akin ang itaas na bahagi. Ung parte na may mga dahon ha?” nakangising sabi ni Matsing.\n“Ha? sa akin ang ibabang bahagi?” tanong ni Pagong.\n“Oo, wala akong panahon para magpatubo pa ng dahon ng saging kaya sa akin na lang ang itaas na parte,” sabi ni Matsing."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing4.png",
                    Subtitle = "Umuwing malungkot si Pagong dala ang kalahating bahagi ng saging na may ugat. Samantalang si Matsing ay masayang umuwi dala ang madahon na bahagi ng puno."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing5.png",
                    Subtitle = "Inalagaan ni Pagong ang kanyang halaman. Araw-araw dinidiligan niya ito at nilalagyan ng pataba ang lupa. Ganoon din ang ginawa ni Matsing. Subalit makalipas ang isang linggo, nalanta ang tanim na saging ni Matsing.\nSi Pagong naman ay natuwa nang makita ang umuusbong na dahon sa puno ng saging. Lalo nitong inalaagaan ang tanim hanggang sa mamunga ito nang hitik na hitik."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing6.png",
                    Subtitle = "Nainggit si Matsing nang makita ang bunga ng saging sa halaman ni Pagong.\n“Aba, nagkabunga ang tanim mo. Paano nangyari iyon? Ang aking tanim ay nalanta at natuyo,” sabi ni Matsing.\n“Inalagaan ko kasi ito ng mabuti. Sabi ni Mang Islaw Kalabaw, malaki ang pag-asang tutubo ang bahagi ng halaman na pinutol kung ito ay may ugat,” paliwanag ni Pagong."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing6.png",
                    Subtitle = "“Hmp! Kaya pala nalanta ang aking tanim,” nanggigil na sambit ni Matsing.\n“Mukhang hinog na ang mga bunga nito. Halika, kunin natin,” anyaya nito.\n“Gusto ko sana kaya lang masyadong mataas ang mga bunga. Hindi ko kayang akyatin,” sabi ni Pagong.\n“Kung gusto mo, ako na lang ang aakyat, ibibigay ko sa iyo ang lahat ng mga bunga. Basta’t bigyan mo lang ako ng konti para sa aking miryenda,” sabi ni Matsing."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing7.png",
                    Subtitle = "Pumayag si Pagong sa alok ni Matsing. Subalit nang makarating na si Matsing sa taas ng puno. Kinain niya lahat ng bunga ng puno. Wala itong itinira para kay Pagong.\n“Akin na lahat ito Pagong. Gutom na gutom na ako. Kulang pa ito para sa akin. Hahaha!” tuwang-tuwang sabi ni Matsing."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing8.png",
                    Subtitle = "Nanatili sa itaas ng puno si Matsing at nakatulog sa sobrang kabusugan."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing9.png",
                    Subtitle = "Galit na galit si Pagong sa ginawa ni Matsing. Habang natutulog ito, naglagay siya ng mga tinik sa ilalim ng puno. Nang magising si Matsing ay nakita niya ang mga tinik kaya’t humingi ito ng tulong kay Pagong.\n“Pagong, tulungan mo ako! Alisin mo ang mga tinik na ito. Malapit ng dumilim at mukhang uulan ng malakas,” pagmamakaawa ni Matsing.\n“Ayoko! Napakasalbahe mo. Lagi mo na lang akong iniisahan! Aalis muna ako. Mukhang malakas ang ulan. Sa bahay ni Aling Muning muna ako habang umuulan.” Sabi ni Pagong sabay alis papunta sa bahay ni Aling Muning."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing10.png",
                    Subtitle = "Makalipas ang ilang sandali, nagsimulang bumuhos ang malakas na ulan. Walang nagawa si Matsing kundi bumaba sa puno ng saging.\n“Arrrraayyy! Aaaarayy! natutusok ako sa mga tinik Arrrrrrrrruuyyyyyy!!!!” daing ng tusong matsing.\n“Humanda ka bukas Pagong. Gaganti ako sa ginawa mo sa akin,” bulong nito sa sarili."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing11.png",
                    Subtitle = "Kinabukasan, kahit mahapdi pa rin ang mga sugat ni Matsing ay hinanap niya si Pagong. Nakita niya itong naglalakad sa may kakahuyan.\n“Hoy Pagong humanda ka ngayon!” galit na sabi ni Matsing sabay huli sa pagong.\n“Anong gagawin mo sa akin?” takot na tanong ni Pagong"
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing11.png",
                    Subtitle = "“Tatadtarin kita ng pinong pino,” sabi ni Matsing.\nNag-isip ng paraan si Pagong para maisahan ang tusong matsing.\n“Oo sige tadtarin mo ako ng pinong-pino at pagputul-putullin nang sa gayon ako ay dadami at susugurin ka namin ng mga parte ng katawan kong pinutol mo hahaha,” sabi ni Pagong."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing12.png",
                    Subtitle = "Nag-isip ng malalin si Matsing.\n“Haha, susunugin na lang kita hanggang sa maging abo ka,” sabi ni Matsing.\n“Hindi ka ba nag-iisip Matsing? Hindi kami tinatablan ng apoy! Nakikita mo ba ang makapal at matibay kong bahay? Kahit ang pinakamatinding apoy ay walang panama dito,” pagyayabang ni Pagong."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing13.png",
                    Subtitle = "Nag-isip na naman ng malalim si Matsing. Hanggang sa maisipan niyang pumunta sa dalampasigan.\n“Tignan natin kung saan ang tapang mo. Itatapon kita dito sa dalampasigan hanggang sa malunod ka! Hahaha!” sabi ni Matsing"
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing14.png",
                    Subtitle = "Lihim na natuwa si Pagong. Nagpanggap itong takot sa dalampasigan.\n“Naku huwag mo akong itatapon sa dalampasigan. Takot ako sa tubig at hindi ako marunong lumangoy. Parang awa mo na…” pagmamakaawa ni Pagong."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing15.png",
                    Subtitle = "Tuwang-tuwa si Matsing sa pag-aakalang magagantihan na niya si Pagong. Todo lakas niya itong itinapon sa dalampasigan. Nagulat ito nang makitang marunong lumangoy si Pagong. Ang bilis-bilis ng pagkilos ni Pagong sa tubig. Kung mabagal ito sa lupa ay parang ang gaan ng katawan nito sa tubig."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing15.png",
                    Subtitle = "“Hahaha. Naisahan din kita Matsing. Hindi mo ba alam na gustong-gusto ko ang lumagoy sa dalampasigan at magbabad sa tubig? Salamat kaibigan!!!” natutuwang sabi ni Pagong.\nMalungkot na umuwi si Matsing. Naisip niya na napakasakit pala na maisahan ng isang kaibigan. Naramdaman niya kung paano masaktan kapag naloloko ng isang kaibigan."
                },
                new Slide{
                    Background = "storiespabula/pagongatmatsing/pagong_at_matsing16.png",
                    Subtitle = "Mula noon nagbago na si Matsing. Hindi na sila muling nagkita ni Pagong."
                }
            },
            Quiz = new()
                {
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    },
                    new QuizQuestion{
                        Prompt="Sino ang pangunahing tauhan?",
                        ChoiceImages=new(){ "stories/juantamad/quiz1/juantamad_quiz1a.png", "stories/juantamad/quiz1/juantamad_quiz1b.png", "stories/juantamad/quiz1/juantamad_quiz1c.png" },
                        CorrectIndex=0, TimeLimitSec=20
                    }
                }
        },
        new Story{
                StoryIndex =34,Category = "pabula", MedalId = 14,
                Id="3_maton", Title="Alamat ng Maton", PriceStars=2, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/maton/s1.png", Subtitle="" } }, // no subtitle example
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },
        new Story{
                StoryIndex =35,Category = "pabula", MedalId = 15,
                Id="3_maton", Title="Alamat ng Maton", PriceStars=2, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/maton/s1.png", Subtitle="" } }, // no subtitle example
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },

        };

        // ------- In-memory runtime (no DB yet) -------
        public static int Stars { get; set; } = 50;
        public static int Hearts { get; set; } = 3;  // daily refill later
        public static string SelectedNarratorId { get; set; } = "tarsier";
        public static string CurrentNarratorImage { get; set; } = "elements/tarsier.png"; // Global variable for narrator image
        
        // Narrator battery system
        public static int NarratorBattery { get; set; } = 3; // 3 = full battery
        public static DateTime LastNarratorUseTime { get; set; } = DateTime.MinValue;
        private static System.Threading.Timer? _narratorBatteryTimer;
        
        public static HashSet<string> UnlockedStories { get; } = new() { "1_juan_tamad" };
        public static HashSet<string> UnlockedNarrators { get; } = new() { "tarsier" };
        public static string category { get; set; } = "";

        // Narrator battery helper methods
        public static string GetNarratorBatteryImage()
        {
            return NarratorBattery switch
            {
                3 => "batteryfull.png",
                2 => "batterythreequarters.png",
                1 => "batteryquarter.png",
                _ => "batteryempty.png"
            };
        }

        public static bool CanUseNarrator()
        {
            CheckAndRefreshNarratorBattery();
            return NarratorBattery > 0;
        }

        public static async Task<bool> UseNarratorAsync()
        {
            if (!CanUseNarrator())
                return false;

            NarratorBattery--;
            LastNarratorUseTime = DateTime.Now;
            
            // Save to database
            await App.Database.UpdateNarratorBatteryAsync(NarratorBattery, LastNarratorUseTime);
            
            // Start 10-minute timer if battery is depleted
            if (NarratorBattery == 0)
            {
                StartNarratorBatteryRefreshTimer();
            }
            
            return true;
        }

        public static void CheckAndRefreshNarratorBattery()
        {
            if (NarratorBattery >= 3)
                return;

            if (LastNarratorUseTime == DateTime.MinValue)
                return;

            var timeSinceLastUse = DateTime.Now - LastNarratorUseTime;
            
            // Check if 10 minutes have passed since last use
            if (timeSinceLastUse.TotalMinutes >= 10)
            {
                // Reset to full battery after 10 minutes
                NarratorBattery = 3;
                LastNarratorUseTime = DateTime.MinValue; // Reset timer
                
                // Save to database asynchronously
                _ = Task.Run(async () => 
                {
                    try
                    {
                        await App.Database.UpdateNarratorBatteryAsync(NarratorBattery, LastNarratorUseTime);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to update narrator battery: {ex.Message}");
                    }
                });
                
                // Stop the timer since battery is full
                _narratorBatteryTimer?.Dispose();
                _narratorBatteryTimer = null;
            }
        }

        private static void StartNarratorBatteryRefreshTimer()
        {
            // Cancel existing timer if any
            _narratorBatteryTimer?.Dispose();
            
            // Create timer that checks every 30 seconds for better responsiveness
            _narratorBatteryTimer = new System.Threading.Timer(
                callback: _ => 
                {
                    CheckAndRefreshNarratorBattery();
                    
                    // If battery is full, dispose the timer
                    if (NarratorBattery >= 3)
                    {
                        _narratorBatteryTimer?.Dispose();
                        _narratorBatteryTimer = null;
                    }
                },
                state: null,
                dueTime: TimeSpan.FromSeconds(30),
                period: TimeSpan.FromSeconds(30)
            );
        }

        public static void InitializeNarratorBatteryTimer()
        {
            CheckAndRefreshNarratorBattery();
            
            if (NarratorBattery < 3)
            {
                StartNarratorBatteryRefreshTimer();
            }
        }

        // Prefer checked sets but also fall back to story fields and price.
        public static bool IsStoryUnlocked(string id)
        {
            // fast-set check first
            if (UnlockedStories.Contains(id)) return true;

            // fallback: check story flags / free price
            var s = Stories.FirstOrDefault(x => x.Id == id);
            if (s == null) return false;

            bool unlocked = s.PriceStars == 0 || s.IsPurchased;

            // If story is unlocked by flags, ensure the HashSet mirrors that
            if (unlocked)
                UnlockedStories.Add(id);

            return unlocked;
        }

        private static readonly HashSet<string> _narratorDbCheckInFlight = new();
        public static string CurrentStoryId { get; set; } = "";
        public static bool MusicIsEnabled { get; set; } = true;
        
        public static bool IsNarratorUnlocked(string id)
        {
            // 'tarsier' is always available
            if (id == "tarsier") return true;

            // Fast in-memory check first
            if (UnlockedNarrators.Contains(id)) return true;

            // Fallback: check per-story monitored flags already in memory
            if (id == "eagle")
            {
                var currentStory = Stories.FirstOrDefault(s => s.Id == CurrentStoryId);
                return currentStory != null && currentStory.NarratorEagleUnlocked;
            }
            if (id == "monkey")
            {
                var currentStory = Stories.FirstOrDefault(s => s.Id == CurrentStoryId);
                return currentStory != null && currentStory.NarratorMonkeyUnlocked;
            }

            if (!_narratorDbCheckInFlight.Contains(id))
            {
                _narratorDbCheckInFlight.Add(id);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var dbHas = await App.Database.IsAnyStoryNarratorUnlockedAsync(id);
                        if (dbHas)
                        {
                            UnlockedNarrators.Add(id);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"IsNarratorUnlocked DB check failed for {id}: {ex}");
                    }
                    finally
                    {
                        _narratorDbCheckInFlight.Remove(id);
                    }
                });
            }

            return false;
        }

        // ------- Helpers -------
        public static Narrator CurrentNarrator => Narrators.First(n => n.Id == SelectedNarratorId);
        public static Story GetStory(string id) => Stories.First(s => s.Id == id);

        public static double NarratorVolume { get; set; } = 1.0;
        public static bool TrySpendStars(int amount)
        {
            if (CharacterHelper.CurrentStars < amount) return false;
            return true;
        }
    }
}
