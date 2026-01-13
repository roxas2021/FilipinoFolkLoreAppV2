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
            new Narrator{ Id="tarsier", Name="Tarsier", Avatar="elements/tarsier.png", PriceStars=0 },
            new Narrator{ Id="eagle",   Name="Agila",   Avatar="elements/eagle.png",   PriceStars=50 },
            new Narrator{ Id="monkey",  Name="Unggoy",  Avatar="elements/monkey.png",  PriceStars=100 },
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
                StoryIndex=1, Category = "alamat",
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
                        Subtitle="Una, nagkwento si Juan Tamad kung paano siya naging tamad sa pagtinda ng puto ng kanyang ina. Sa sobrang init ng panahon, nagpasya siyang magpahinga na lamang kaysa tignan ang kalidad ng kanyang tinitindang produkto. Inabot siya ng antok at hindi napansin na kinain na pala ng mga palakang gutom ang lahat ng kanyang puto. Nang makarating siya sa bahay, kinailangan niyang magtago ng dalawang palakang gutom upang hindi siya mapagalitan ng kanyang ina."
                    },
                    new Slide{
                        Background="stories/juantamad/juan_tamad12.PNG",
                        Subtitle="Ikalawa, inihayag ni Juan Tamad ang kwento tungkol sa pagtanggap ng kanyang ina ng trabaho na magbenta ng palayok sa palengke. Dahil sa kagustuhang kumita ng pera, nagdesisyon siyang maglakad papunta sa palengke nang may dalang palayok. Sa daan, nakasalubong niya si Mariang Masipag na nagmamaneho ng bisikleta. Dahil sa antok at kaantukan, nabangga ni Juan si Mariang Masipag, at nasira ang mga bitbit nitong palayok. Dahil doon, napilitan siyang gumawa ng paraan para magkaroon ng pambayad: binayaran niya ang mga palayok nang pino-pino, itinaga sa malambot na dahon, at ipinakalat bilang “gamot sa galis.”"
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
                StoryIndex =2,Category = "alamat",
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
                StoryIndex =3,Category = "alamat",
                Id="3_maton", Title="Alamat ng Maton", PriceStars=2, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/maton/s1.png", Subtitle="" } }, // no subtitle example
                Quiz=new(){ new QuizQuestion{ Prompt="Sino ang bida?",
                           ChoiceImages=new(){ "quiz/maton/a.png","quiz/maton/b.png","quiz/maton/c.png"},
                           CorrectIndex=2 } }
            },
            new Story{
                StoryIndex=4,Category = "alamat",
                Id="4_mangga", Title="Alamat ng Mangga", PriceStars=3, RewardStars=20,
                Slides=new(){ new Slide{ Background="stories/mangga/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong prutas?",
                           ChoiceImages=new(){ "quiz/mangga/a.png","quiz/mangga/b.png","quiz/mangga/c.png"},
                           CorrectIndex=1 } }
            },
            new Story{
                StoryIndex= 5,Category = "alamat",
                Id="5_saging", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },
            new Story{
                StoryIndex =6,Category = "alamat",
                Id="5_luya", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },new Story{
                StoryIndex = 6,Category = "alamat",
                Id="5_kamatis", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },new Story{
                StoryIndex =7,Category = "alamat",
                Id="5_bawang", Title="Alamat ng Saging", PriceStars=50, RewardStars=25,
                Slides=new(){ new Slide{ Background="stories/saging/s1.png", Subtitle="..." } },
                Quiz=new(){ new QuizQuestion{ Prompt="Anong halaman?",
                           ChoiceImages=new(){ "quiz/saging/a.png","quiz/saging/b.png","quiz/saging/c.png"},
                           CorrectIndex=0 } }
            },
            //Epiko
            new Story{
                StoryIndex =8,Category = "epiko",
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
                    }
                }
            },
            new Story{
                StoryIndex = 9,Category = "pabula",
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

        };

        // ------- In-memory runtime (no DB yet) -------
        public static int Stars { get; set; } = 50;
        public static int Hearts { get; set; } = 3;  // daily refill later
        public static string SelectedNarratorId { get; set; } = "tarsier";
        public static HashSet<string> UnlockedStories { get; } = new() { "1_juan_tamad" };
        public static HashSet<string> UnlockedNarrators { get; } = new() { "tarsier" };
        public static string category { get; set; } = "";
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
        // AlamatContent.cs (inside AlamatContent static class)

        private static readonly HashSet<string> _narratorDbCheckInFlight = new();
        public static string CurrentStoryId { get; set; } = "";

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


            // If we haven't already launched a DB check for this narrator, start one in background.
            // This will update UnlockedNarrators if DB says any story has it unlocked.
            // We guard by _narratorDbCheckInFlight so we only query once concurrently per narrator.
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
                            // Update the in-memory set for fast lookups later
                            UnlockedNarrators.Add(id);

                            // Optionally: if you want UI to refresh immediately, you can
                            // publish a message here (MessagingCenter) or call a UI refresh handler.
                            // Example (uncomment if you use MessagingCenter):
                            // MessagingCenter.Send<object, string>(this, "NarratorUnlocked", id);
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

            // still locked for now (until DB check completes or a story flag shows unlock)
            return false;
        }


        // ------- Helpers -------
        public static Narrator CurrentNarrator => Narrators.First(n => n.Id == SelectedNarratorId);
        public static Story GetStory(string id) => Stories.First(s => s.Id == id);


        public static bool TrySpendStars(int amount)
        {
            if (CharacterHelper.CurrentStars < amount) return false;
            return true;
        }
    }
}
