# Act I — in-game script

Reading copy exported from `game/data/Map001.json`, `Map002.json`, and `Map003.json`. Events appear in map order; event pages may depend on switches. The map data is authoritative.

## Act1_Cemetery

### Event 01 · OpeningController · (0,0)

**Page 1**

- **GRACE** — Everyone wore black today. Even Uncle James wore a tie.
- **MOM** — Grandma's stone is up the path, where the white flowers are. I'll be here.

### Event 02 · Mom · (13,12)

**Page 1**

- **MOM** — Take your time, Gracie. I can see you from here.

**Page 2 · MomReadyAfterFlashback**

- **MOM** — We found this in Grandma's house. See? She wrote your name on it.
- **GRACE** — For Grace. It feels heavier than it looks.
- **MOM** — You can open it by the bench. The lid has a little winding key.

**Page 3 · MusicBoxReceived**

- **MOM** — I'll wait right here, sweetheart.

### Event 03 · Dad · (14,7)

**Page 1**

- **GRACE** — Dad?
- **DAD** — ...
- **GRACE** — He's looking at Grandma's stone.

**Page 2 · VisitedGrave**

- **GRACE** — Dad?
- **GRACE** — Dad.
- **DAD** — Hey, Gracie.
- **GRACE** — Are you crying?
- **DAD** — A little.
- **GRACE** — Why?
- **DAD** — Because I miss Grandma.
- **GRACE** — Oh.

**Page 3 · FlashbackComplete**

- **DAD** — I used to know what to say to her. I wish I'd asked her to sit with me.

### Event 04 · Gravestone_Main · (11,5)

**Page 1**

- **GRACE** — That's Grandma's name. Mom taught me to read it.
- **GRACE** — She is home now.
- **GRACE** — Her picture doesn't look like her. She looks younger.
- **GRACE** — She knew my name then.
- **GRACE** — Dad's wiping his face.

**Page 2 · VisitedGrave**

- **GRACE** — She is home now.

### Event 05 · WhiteFlowers · (10,5)

**Page 1**

- **GRACE** — There are so many flowers.
- **GRACE** — Grandma didn't like strong smells. Flowers made her sneeze.
- **GRACE** — I don't remember when she stopped saying that.
- **GRACE** — Flowers made her sneeze.

### Event 06 · OldGrave_WitheredFlowers · (5,9)

**Page 1**

- **GRACE** — The petals have curled up. Someone tied the stems so they would stay together.

### Event 07 · UncleJames · (8,9)

**Page 1**

- **UNCLE JAMES** — She kept a spare tie in the hall. Said you never know when you might need one.

### Event 08 · Aunt · (17,10)

**Page 1**

- **AUNT** — Those white flowers used to grow by her door. She let you pick them even when it rained.

### Event 09 · Bench_RelicBox · (6,12)

**Page 1**

- **GRACE** — Mom's bag is by the bench. There is a little box, too.

### Event 10 · AfterFlashback_EndAct · (1,0)

**Page 2 · FlashbackComplete**

- **DAD** — Grace? You okay?
- **GRACE** — I was thinking about home.
- **DAD** — Me too.
- **MOM** — Grace? Come here a second. I have something for you.

### Event 11 · ExitLeft · (11,16)

**Page 1**

- **GRACE** — I should stay where Mom can see me.

### Event 12 · ExitRight · (12,16)

**Page 1**

- **GRACE** — I should stay where Mom can see me.

### Event 13 · Gravestone_Right · (12,5)

**Page 1**

- **GRACE** — That's Grandma's name. Mom taught me to read it.
- **GRACE** — She is home now.
- **GRACE** — Her picture doesn't look like her. She looks younger.
- **GRACE** — She knew my name then.
- **GRACE** — Dad's wiping his face.

**Page 2 · VisitedGrave**

- **GRACE** — She is home now.

### Event 15 · Bench_RelicBox · (5,12)

**Page 1**

- **GRACE** — Mom's bag is by the bench. There is a little box, too.

### Event 16 · OldGrave_3 · (5,4)

**Page 1**

- **GRACE** — Moss has covered part of the name. There are little purple flowers at the edge.

### Event 17 · OldGrave_4 · (18,4)

**Page 1**

- **GRACE** — There is a sprig of rosemary on the dark stone. Grandma used to rub it between her fingers.

### Event 18 · WhiteFlowers · (13,5)

**Page 1**

- **GRACE** — There are so many flowers.
- **GRACE** — Grandma didn't like strong smells. Flowers made her sneeze.
- **GRACE** — I don't remember when she stopped saying that.
- **GRACE** — Flowers made her sneeze.

### Event 19 · OldGrave_2 · (18,9)

**Page 1**

- **GRACE** — Three little pebbles, all in a row. Someone must come back here to put them straight.

### Event 20 · Grandpa · (16,7)

**Page 1**

- **GRANDPA** — She used to hum in the kitchen. Sometimes I still catch myself waiting for it.

### Event 21 · WindMusicBox · (5,12)

**Page 2 · MusicBoxReceived**

- **GRACE** — There is a picture inside. And a tiny brass key.
- **Choice:** Turn the key / Close the lid

### Event 22 · RibbonAmongChamomile · (8,8)

**Page 1**

- A ribbon, caught low in the chamomile. Grandma tied bows with this same blue.

### Event 23 · Birdbath · (19,12)

**Page 1**

- Rain falls into the little basin. Grandma used to count the birds that came.

### Event 24 · GatePlaque · (10,15)

**Page 1**

- The iron is cold even through my sleeve. We came through here together, last spring.

## Act1_House_Flashback

### Event 01 · FlashbackController · (0,0)

**Page 1 · FlashbackActive**

- **GRANDMA** — I need to go home.
- **DAD** — Mom. You are home.
- **GRANDMA** — No.
- **DAD** — We talked about this.
- **GRANDMA** — Grace?
- **DAD** — Mom, please—
- **GRANDMA** — Let me go home!
- **DAD** — I CAN'T DO THIS AGAIN.

## Act1_Memory_Home

### Event 01 · Grandma · (12,8)

**Page 1**

- **GRANDMA** — Gracie? There you are. I kept the light on.
- **GRACE** — I thought I had lost you. Can I stay with you for a little while?
- **GRANDMA** — Come sit with me, love. We have time for the rest of the song.

**Page 2 · Act1Complete**

- **GRANDMA** — You can tell me what you saw today. Start anywhere you like.

### Event 02 · FamilyPhotograph · (5,7)

**Page 1**

- **GRACE** — That is the picture from the lid. Only here, nobody has faded yet.

**Page 2 · MemoryPhotoCollected**

- I know this picture now. I think I remember that afternoon.

### Event 03 · GrandmasBlanket · (5,10)

**Page 1**

- **GRACE** — The blanket she always put over my knees. One corner is still warm.

### Event 04 · Cabinet · (19,7)

**Page 1**

- **GRACE** — The tall shelf smells like paper and oranges. She hid the good biscuits behind the books.

### Event 05 · Door · (12,4)

**Page 1**

- **GRACE** — There is no rain on the other side. Just the end of the song, still playing.

### Event 06 · Arrival · (0,0)

**Page 1**

- **GRACE** — Grandma's house? I know this song.

### Event 07 · Chair · (18,11)

**Page 1**

- **GRACE** — I used to stand here to see out the window. Grandma would keep a hand at my back.

### Event 08 · TeaCup · (19,11)

**Page 1**

- Her cup is still warm. She always left the good cup for me.

