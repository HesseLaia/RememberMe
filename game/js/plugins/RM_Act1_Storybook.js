/*:
 * @plugindesc Remember Me Act I: storybook interface, exploration camera and interaction cues.
 * @help Use <RMInspect: label> on action events to name their nearby interaction cue.
 */

(function() {
    'use strict';

    var gold = '#e4c893';
    var ink = '#f5edda';
    var scale = 1.32;
    var controls = 'WASD / Arrows  ·  Shift Run  ·  C Crouch';
    Input.keyMapper[65] = 'left';
    Input.keyMapper[68] = 'right';
    Input.keyMapper[83] = 'down';
    Input.keyMapper[87] = 'up';
    Input.keyMapper[67] = 'rmCrouch';
    Input.keyMapper[69] = 'ok';
    Input.keyMapper[73] = 'rmBag';

    function actMap() {
        var id = $gameMap.mapId();
        return id >= 1 && id <= 3;
    }

    function followsPlayer() {
        return $gameMap.mapId() === 1 || $gameMap.mapId() === 3;
    }

    function crouching(player) {
        return followsPlayer() && player.isNormal() && player._rmCrouching &&
            !$gameMap.isEventRunning() && !$gameMessage.isBusy();
    }

    var moveByInput = Game_Player.prototype.moveByInput;
    Game_Player.prototype.moveByInput = function() {
        if (followsPlayer() && this.canMove() && Input.isTriggered('rmCrouch')) {
            this._rmCrouching = !this._rmCrouching;
            if (this._rmCrouching) this._dashing = false;
            $gameTemp.clearDestination();
        }
        moveByInput.call(this);
    };
    var dash = Game_Player.prototype.updateDashing;
    Game_Player.prototype.updateDashing = function() {
        if (!followsPlayer()) return dash.call(this);
        if (!this.isMoving()) {
            this._dashing = this.canMove() && this.isNormal() && !$gameMap.isDashDisabled() &&
                !crouching(this) && (Input.isPressed('shift') || (ConfigManager.alwaysDash && Input.dir4 !== 0));
        }
    };
    var speed = Game_Player.prototype.realMoveSpeed;
    Game_Player.prototype.realMoveSpeed = function() {
        return crouching(this) ? 3.25 : speed.call(this);
    };
    var playerImage = Game_Player.prototype.characterName;
    Game_Player.prototype.characterName = function() {
        var name = playerImage.call(this);
        if (!followsPlayer() || name !== '$RM_Grace') return name;
        if (crouching(this)) return '$RM_Grace_Crouch';
        return this.isMoving() && this.isDashing() && this.isNormal() && !$gameMap.isEventRunning() ?
            '$RM_Grace_Run' : name;
    };
    var locatePlayer = Game_Player.prototype.performTransfer;
    Game_Player.prototype.performTransfer = function() {
        if (this.isTransferring()) this._rmCrouching = false;
        locatePlayer.call(this);
    };

    function goal() {
        if ($gameMap.mapId() === 3) {
            return $gameSwitches.value(5) ? 'Stay a little longer, if you like' : 'Find Grandma in the house';
        }
        if ($gameSwitches.value(9)) return 'Open the music box by the bench';
        if ($gameSwitches.value(4)) return 'Go back to Mom by the path';
        if ($gameSwitches.value(2)) return 'Talk to Dad near the grave';
        return "Follow the path to Grandma's grave";
    }

    function keyForGoal() {
        if ($gameMap.mapId() === 3) return $gameSwitches.value(5) ? [] : [1];
        if ($gameSwitches.value(9)) return [21];
        if ($gameSwitches.value(4)) return [2];
        return $gameSwitches.value(2) ? [3] : [4, 13];
    }

    var screenTileX = Game_Map.prototype.screenTileX;
    var screenTileY = Game_Map.prototype.screenTileY;
    Game_Map.prototype.screenTileX = function() {
        return screenTileX.call(this) / (followsPlayer() ? scale : 1);
    };
    Game_Map.prototype.screenTileY = function() {
        return screenTileY.call(this) / (followsPlayer() ? scale : 1);
    };
    var canvasToMapX = Game_Map.prototype.canvasToMapX;
    var canvasToMapY = Game_Map.prototype.canvasToMapY;
    Game_Map.prototype.canvasToMapX = function(x) {
        return canvasToMapX.call(this, followsPlayer() ? x / scale : x);
    };
    Game_Map.prototype.canvasToMapY = function(y) {
        return canvasToMapY.call(this, followsPlayer() ? y / scale : y);
    };

    function targetX(player) {
        return player._realX - ($gameMap.screenTileX() - 1) * 0.5;
    }

    function targetY(player) {
        return player._realY - $gameMap.screenTileY() * 0.58;
    }

    function drift(current, target, limit, deadZone) {
        var delta = Math.max(0, Math.min(limit, target)) - current;
        if (Math.abs(delta) <= deadZone) return current;
        return current + (delta - Math.sign(delta) * deadZone) * 0.16;
    }

    var centerPlayer = Game_Player.prototype.center;
    Game_Player.prototype.center = function(x, y) {
        if (followsPlayer()) return $gameMap.setDisplayPos(targetX(this), targetY(this));
        return centerPlayer.call(this, x, y);
    };
    var updateScroll = Game_Player.prototype.updateScroll;
    Game_Player.prototype.updateScroll = function(lastX, lastY) {
        if (!actMap()) return updateScroll.call(this, lastX, lastY);
        if (!followsPlayer()) return;
        $gameMap.setDisplayPos(
            drift($gameMap.displayX(), targetX(this), $gameMap.width() - $gameMap.screenTileX(), 0.44),
            drift($gameMap.displayY(), targetY(this), $gameMap.height() - $gameMap.screenTileY(), 0.35)
        );
    };
    var lowerLayer = Spriteset_Map.prototype.createLowerLayer;
    Spriteset_Map.prototype.createLowerLayer = function() {
        lowerLayer.call(this);
        if (!followsPlayer()) return;
        this._baseSprite.scale.set(scale);
        if ($gameMap.mapId() === 1) {
            this._rmCanopy = new Sprite(ImageManager.loadParallax('!RM_CemeteryCanopyStory'));
            this._rmCanopy.z = 5;
            this._tilemap.addChild(this._rmCanopy);
        }
    };
    var updateTilemap = Spriteset_Map.prototype.updateTilemap;
    Spriteset_Map.prototype.updateTilemap = function() {
        updateTilemap.call(this);
        if (this._rmCanopy) {
            this._rmCanopy.x = -Math.floor(this._tilemap.origin.x);
            this._rmCanopy.y = -Math.floor(this._tilemap.origin.y);
        }
    };

    function canInspect(event) {
        return event && event.event().meta.RMInspect && event.isTriggerIn([0]) &&
            event.list() && event.list().length > 1;
    }

    var startEvent = Game_Event.prototype.start;
    Game_Event.prototype.start = function() {
        if (this.event().meta.RMCrouch && !crouching($gamePlayer)) return;
        startEvent.call(this);
    };

    function nearby(player) {
        var direction = player.direction();
        var aheadX = $gameMap.roundXWithDirection(player.x, direction);
        var aheadY = $gameMap.roundYWithDirection(player.y, direction);
        var candidates = $gameMap.eventsXy(aheadX, aheadY).filter(canInspect);
        if (candidates.length) return candidates[0];
        return $gameMap.events().find(function(event) {
            return Math.abs(event.x - player.x) + Math.abs(event.y - player.y) <= 1 &&
                canInspect(event);
        });
    }

    var buttonAction = Game_Player.prototype.triggerButtonAction;
    Game_Player.prototype.triggerButtonAction = function() {
        if (buttonAction.call(this)) return true;
        if (!followsPlayer() || !Input.isTriggered('ok')) return false;
        var event = nearby(this);
        if (!event) return false;
        if (event.event().meta.RMCrouch && !crouching(this)) {
            cueSound('RM_StoryTap', 14, 94, 0);
            return true;
        }
        var horizontal = event.x - this.x;
        var vertical = event.y - this.y;
        if (horizontal || vertical) this.setDirection(horizontal ? (horizontal > 0 ? 6 : 4) : (vertical > 0 ? 2 : 8));
        event.start();
        return $gameMap.setupStartingEvent();
    };

    function cueSound(name, volume, pitch, pan) {
        AudioManager.playSe({name: name, volume: volume, pitch: pitch, pan: pan});
    }
    var lastStep = 0;
    var increaseSteps = Game_Player.prototype.increaseSteps;
    Game_Player.prototype.increaseSteps = function() {
        increaseSteps.call(this);
        if (!followsPlayer() || !this.isNormal() || this.isMoveRouteForcing() || $gameMap.isEventRunning()) return;
        var surface = this.regionId();
        lastStep = (lastStep + 1 + Math.floor(Math.random() * 2)) % 3;
        var material = surface === 2 ? 'Stone' : surface >= 3 ? 'Wood' : 'Grass';
        var volume = crouching(this) ? 10 : this.isDashing() ? 23 : 16;
        cueSound('RM_Story' + material, surface === 4 ? Math.floor(volume * 0.65) : volume,
            94 + lastStep * 5, 0);
    };

    function lettering(bitmap, value, x, y, width, height, size, color, align) {
        bitmap.fontFace = 'Georgia, serif';
        bitmap.fontSize = size;
        bitmap.outlineWidth = 0;
        bitmap.textColor = color;
        bitmap.drawText(value, x, y, width, height, align || 'left');
    }

    function plate(bitmap, width, height) {
        bitmap.clear();
        bitmap.fillRect(0, 0, width, height, 'rgba(15,27,28,0.91)');
        bitmap.fillRect(0, 0, width, 1, 'rgba(228,200,147,0.55)');
        bitmap.fillRect(0, height - 1, width, 1, 'rgba(228,200,147,0.4)');
        bitmap.fillRect(0, 0, 2, height, gold);
    }

    var displayObjects = Scene_Map.prototype.createDisplayObjects;
    Scene_Map.prototype.createDisplayObjects = function() {
        displayObjects.call(this);
        if (!followsPlayer()) return;
        this._rmGoal = new Sprite(new Bitmap(310, 59));
        this._rmGoal.move(20, 18);
        this._rmPrompt = new Sprite(new Bitmap(306, 36));
        this._rmMarker = new Sprite(new Bitmap(22, 19));
        this._rmMarker.anchor.set(0.5, 1);
        lettering(this._rmMarker.bitmap, '◆', 0, 0, 22, 19, 14, gold, 'center');
        this.addChildAt(this._rmMarker, this.getChildIndex(this._windowLayer));
        this.addChildAt(this._rmGoal, this.getChildIndex(this._windowLayer));
        this.addChildAt(this._rmPrompt, this.getChildIndex(this._windowLayer));
        this._rmGoal.visible = this._rmPrompt.visible = this._rmMarker.visible = false;
    };

    var updateMap = Scene_Map.prototype.update;
    Scene_Map.prototype.update = function() {
        updateMap.call(this);
        if (!this._rmGoal) return;
        var active = this.isActive() && $gamePlayer.canMove() && this._messageWindow.isClosed() &&
            !$gameMessage.isBusy() &&
            !($gameMap.mapId() === 1 && !$gameSwitches.value(1));
        this._rmGoal.visible = !!active;
        this._rmPrompt.visible = this._rmMarker.visible = false;
        if (!active) return;
        var objective = goal();
        if (this._rmObjective !== objective) {
            this._rmObjective = objective;
            plate(this._rmGoal.bitmap, 310, 59);
            lettering(this._rmGoal.bitmap, $gameSwitches.value(5) ? 'A C T   I   /   C O M P L E T E' :
                'A C T   I   /   O B J E C T I V E', 14, 6, 280, 17, 11, gold);
            lettering(this._rmGoal.bitmap, objective, 14, 25, 282, 25, 17, ink);
        }
        var event = nearby($gamePlayer);
        var keys = keyForGoal();
        var target = keys.length ? $gameMap.event(keys[0]) : null;
        if (target && (Math.abs(target.x - $gamePlayer.x) + Math.abs(target.y - $gamePlayer.y) <= 6)) {
            this._rmMarker.x = target.screenX() * scale;
            this._rmMarker.y = (target.screenY() - 50) * scale + Math.sin(Graphics.frameCount / 24) * 3;
            this._rmMarker.visible = this._rmMarker.x > 14 && this._rmMarker.x < Graphics.width - 14 &&
                this._rmMarker.y > 85 && this._rmMarker.y < Graphics.height - 18;
        }
        var tip = event ? event.event().meta.RMInspect.trim() : '';
        var wantsCrouch = event && event.event().meta.RMCrouch && !crouching($gamePlayer);
        if (!tip && !$gameSwitches.value(2) && $gameMap.mapId() === 1) tip = controls;
        if (!tip) return;
        this._rmPrompt.visible = true;
        this._rmPrompt.move((Graphics.width - 306) / 2, Graphics.height - 53);
        var label = wantsCrouch ? 'C   /   Crouch to look closer' : event ? 'E / ENTER   ·   ' + tip : tip;
        if (this._rmTip !== label) {
            this._rmTip = label;
            plate(this._rmPrompt.bitmap, 306, 36);
            lettering(this._rmPrompt.bitmap, label, 5, 0, 296, 36, 15, ink, 'center');
        }
    };

    Window_Base.prototype.loadWindowskin = function() {
        this.windowskin = ImageManager.loadSystem('RM_StoryPaper');
    };
    Window_Base.prototype.standardBackOpacity = function() {
        return 235;
    };
    var paperFont = Window_Base.prototype.resetFontSettings;
    Window_Base.prototype.resetFontSettings = function() {
        paperFont.call(this);
        this.contents.outlineWidth = 0;
    };
    var bootWindowskin = Scene_Boot.prototype.loadSystemWindowImage;
    Scene_Boot.prototype.loadSystemWindowImage = function() {
        bootWindowskin.call(this);
        ImageManager.reserveSystem('RM_StoryPaper');
    };

    Window_Message.prototype.windowWidth = function() {
        return Math.min(700, Graphics.boxWidth - 44);
    };
    Window_Message.prototype.standardFontFace = function() {
        return 'Georgia, serif';
    };
    Window_Message.prototype.standardFontSize = function() {
        return 23;
    };
    Window_Message.prototype.standardPadding = function() {
        return 17;
    };
    Window_Message.prototype.newLineX = function() {
        return $gameMessage.faceName() ? 128 : 10;
    };
    Window_Message.prototype.drawMessageFace = function() {
        var index = $gameMessage.faceIndex();
        this.contents.fillRect(7, 7, 112, 112, '#aa9470');
        this.contents.blt(this._faceBitmap, index % 4 * 144, Math.floor(index / 4) * 144,
            144, 144, 9, 9, 108, 108);
    };
    var messagePlacement = Window_Message.prototype.updatePlacement;
    Window_Message.prototype.updatePlacement = function() {
        messagePlacement.call(this);
        if (this._positionType === 2) this.y -= 18;
        if (this._positionType === 0) this.y += 18;
    };
    var titleList = Window_TitleCommand.prototype.makeCommandList;
    Window_TitleCommand.prototype.makeCommandList = function() {
        titleList.call(this);
        this._list[0].name = 'New Game';
        this._list[1].name = 'Continue';
        this._list[2].name = 'Settings';
    };
    Window_TitleCommand.prototype.windowWidth = function() { return 456; };
    Window_TitleCommand.prototype.maxCols = function() { return 3; };
    Window_TitleCommand.prototype.numVisibleRows = function() { return 1; };
    Window_TitleCommand.prototype.itemTextAlign = function() { return 'center'; };
    Window_TitleCommand.prototype.standardFontFace = function() { return 'Georgia, serif'; };
    Window_TitleCommand.prototype.standardFontSize = function() { return 22; };
    var titleCommands = Scene_Title.prototype.createCommandWindow;
    Scene_Title.prototype.createCommandWindow = function() {
        titleCommands.call(this);
        this._commandWindow.x = Math.round((Graphics.boxWidth - this._commandWindow.width) / 2);
        this._commandWindow.y = Graphics.boxHeight - 96;
    };

    var menuCommands = Window_MenuCommand.prototype.makeCommandList;
    Window_MenuCommand.prototype.makeCommandList = function() {
        if (!actMap()) return menuCommands.call(this);
        this.addCommand('Resume', 'resume');
        this.addCommand('Inventory', 'rmBag');
        this.addSaveCommand();
        this.addOptionsCommand();
        this.addGameEndCommand();
    };
    var createMenu = Scene_Menu.prototype.create;
    Scene_Menu.prototype.create = function() {
        if (!actMap()) return createMenu.call(this);
        Scene_MenuBase.prototype.create.call(this);
        var heading = new Sprite(new Bitmap(Graphics.width, 104));
        heading.y = 122;
        lettering(heading.bitmap, 'P A U S E D', 0, 0, Graphics.width, 54, 40, ink, 'center');
        lettering(heading.bitmap, goal(), 0, 60, Graphics.width, 32, 18, gold, 'center');
        this.addChildAt(heading, this.getChildIndex(this._windowLayer));
        this.createCommandWindow();
        this._commandWindow.setHandler('resume', this.popScene.bind(this));
        this._commandWindow.setHandler('rmBag', function() { SceneManager.push(Scene_RMInventory); });
        this._commandWindow.x = Math.round((Graphics.boxWidth - this._commandWindow.width) / 2);
        this._commandWindow.y = 254;
    };
    var startMenu = Scene_Menu.prototype.start;
    Scene_Menu.prototype.start = function() {
        if (!actMap()) return startMenu.call(this);
        Scene_MenuBase.prototype.start.call(this);
    };

    function Window_RMInventory() { this.initialize.apply(this, arguments); }
    Window_RMInventory.prototype = Object.create(Window_Selectable.prototype);
    Window_RMInventory.prototype.constructor = Window_RMInventory;
    Window_RMInventory.prototype.initialize = function(x, y, width, height) {
        Window_Selectable.prototype.initialize.call(this, x, y, width, height);
        this._items = $gameParty.items().filter(function(item) { return item.itypeId === 2; });
        this.refresh();
        this.select(0);
        this.activate();
    };
    Window_RMInventory.prototype.maxItems = function() {
        return Math.max(1, this._items ? this._items.length : 0);
    };
    Window_RMInventory.prototype.standardFontFace = function() { return 'Georgia, serif'; };
    Window_RMInventory.prototype.standardFontSize = function() { return 23; };
    Window_RMInventory.prototype.drawItem = function(index) {
        var item = this._items[index];
        var rect = this.itemRectForText(index);
        this.drawText(item ? item.name : 'No keepsakes yet', rect.x + 8, rect.y, rect.width - 12);
    };
    Window_RMInventory.prototype.updateHelp = function() {
        var item = this._items[this.index()];
        if (!item) this._helpWindow.setText('Keepsakes you find will appear here.');
        else this._helpWindow.setItem(item);
    };

    function Scene_RMInventory() { this.initialize.apply(this, arguments); }
    Scene_RMInventory.prototype = Object.create(Scene_MenuBase.prototype);
    Scene_RMInventory.prototype.constructor = Scene_RMInventory;
    Scene_RMInventory.prototype.create = function() {
        Scene_MenuBase.prototype.create.call(this);
        this.setBackgroundOpacity(128);
        var header = new Sprite(new Bitmap(Graphics.width, 100));
        header.y = 69;
        lettering(header.bitmap, 'K E E P S A K E S', 0, 0, Graphics.width, 58, 37, ink, 'center');
        lettering(header.bitmap, 'INVENTORY   /   ESC TO GO BACK', 0, 58, Graphics.width, 24, 14, gold, 'center');
        this.addChildAt(header, this.getChildIndex(this._windowLayer));
        var width = Math.min(580, Graphics.boxWidth - 40);
        var edge = Math.floor((Graphics.boxWidth - width) / 2);
        this._list = new Window_RMInventory(edge, 172, width, 205);
        this._description = new Window_Help(3);
        this._description.move(edge, 387, width, this._description.height);
        this._description.standardFontFace = function() { return 'Georgia, serif'; };
        this._description.standardFontSize = function() { return 21; };
        this._description.createContents();
        this._list.setHelpWindow(this._description);
        this._list.setHandler('cancel', this.popScene.bind(this));
        this.addWindow(this._list);
        this.addWindow(this._description);
        this._list.updateHelp();
    };

    var updateScene = Scene_Map.prototype.update;
    Scene_Map.prototype.update = function() {
        updateScene.call(this);
        if (followsPlayer() && this.isActive() && !SceneManager.isSceneChanging() &&
            $gamePlayer.canMove() && !$gamePlayer.isMoving() && Input.isTriggered('rmBag')) {
            SoundManager.playOk();
            SceneManager.push(Scene_RMInventory);
        }
    };
})();
