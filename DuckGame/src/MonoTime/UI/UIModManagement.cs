using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace DuckGame
{
	public class UIModManagement : UIMenu
	{
		public override void Close()
		{
			if (!fixView)
			{
				_showingMenu = false;
				_editModMenu.Close();
                Layer.HUD.camera.width /= 2;
				Layer.HUD.camera.height /= 2;
				fixView = true;
				DevConsole.RestoreDevConsole();
			}

			base.Close();
		}

		// callback stuff
		void EnableDisableMod()
		{
			_awaitingChanges = true;

			if (_selectedMod.configuration.disabled)
				_selectedMod.configuration.Enable();
			else
				_selectedMod.configuration.Disable();

            modsChanged = true;

			_editModMenu.Close();
			Open();
        }

		private const int FO_DELETE = 0x0003;
		private const int FOF_ALLOWUNDO = 0x0040;           // Preserve undo information, if possible. 
		private const int FOF_NOCONFIRMATION = 0x0010;      // Show no confirmation dialog box to the user

		// Struct which contains information that the SHFileOperation function uses to perform file operations. 
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct SHFILEOPSTRUCT
		{
			public IntPtr hwnd;
			[MarshalAs(UnmanagedType.U4)]
			public int wFunc;
			public string pFrom;
			public string pTo;
			public short fFlags;
			[MarshalAs(UnmanagedType.Bool)]
			public bool fAnyOperationsAborted;
			public IntPtr hNameMappings;
			public string lpszProgressTitle;
		}

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

		static void DeleteFileOrFolder(string path)
		{
			SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
			fileop.wFunc = FO_DELETE;
			fileop.pFrom = path + '\0' + '\0';
			fileop.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
			SHFileOperation(ref fileop);
		}

		void DeleteMod()
		{
			ShowYesNo(_editModMenu, () =>
			{
				_awaitingChanges = true;

                if (_selectedMod.configuration.workshopID == 0)
					DeleteFileOrFolder(_selectedMod.configuration.directory);
				else
					Steam.WorkshopUnsubscribe(_selectedMod.configuration.workshopID);

				_mods.Remove(_selectedMod);
				_hoverIndex = -1;

				_yesNoMenu.Close();
				_editModMenu.Close();
				Open();
            });
        }

		UIMenu _openOnClose;
		Sprite _moreArrow, _noImage, _steamIcon;
		SpriteMap _cursor, _localIcon, _newIcon, _settingsIcon;
		IList<Mod> _mods;
		int _hoverIndex = 0;
		UIBox _box;
		FancyBitmapFont _fancyFont;
		int _maxModsToShow = 0;
		UIMenuItem _uploadItem, _disableOrEnableItem, _deleteOrUnsubItem, _visitItem;

		public UIMenu _editModMenu, _yesNoMenu;
		UIMenuItem _yesNoYes, _yesNoNo;


		SteamUploadDialog _uploadDialog;
		WorkshopItem _transferItem;
		bool _transferring = false;
		bool _awaitingChanges = false;
		Textbox _updateTextBox;
		Rectangle _updateButton;
		string _updateButtonText = "UPDATE MOD!";
		int _pressWait;
        Sprite _modErrorIcon;

		void ShowYesNo(UIMenu goBackTo, UIMenuActionCallFunction.Function onYes)
		{
			_yesNoNo.menuAction = new UIMenuActionCallFunction(() =>
			{
				_yesNoMenu.Close();
				goBackTo.Open();
            });

			_yesNoYes.menuAction = new UIMenuActionCallFunction(onYes);
			
			UIMenuActionOpenMenu ac = new UIMenuActionOpenMenu(_editModMenu, _yesNoMenu);
			ac.Activate();
		}

		void UploadMod()
		{
			_editModMenu.Close();
			Open();

			//Layer.HUD.camera.width *= 2;
			//Layer.HUD.camera.height *= 2;
			////MonoMain.pauseMenu = null;
			//_uploadDialog.Open(_selectedMod);
			//return;





			if (_selectedMod.configuration.workshopID == 0)
				_transferItem = Steam.CreateItem();
			else
			{
				_transferItem = new WorkshopItem(_selectedMod.configuration.workshopID);
				_needsUpdateNotes = true;
                _updateTextBox.GainFocus();
				_gamepadMode = false;
            }

			_transferring = false;
		}

		void VisitModPage()
		{
			_editModMenu.Close();
			Open();
			
			Steam.OverlayOpenURL("http://steamcommunity.com/sharedfiles/filedetails/?id=" + _selectedMod.configuration.workshopID);
		}

		static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			// If the destination directory doesn't exist, create it. 
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
                File.SetAttributes(temppath, FileAttributes.Normal);
			}

			// If copying subdirectories, copy them and their contents to new location. 
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
        }

        private sealed class UI_ModSettings : Mod
        {

		}

		private sealed class UI_WorkshopItem : Mod
		{
			public WorkshopItem item;
			public UI_WorkshopItem(WorkshopItem pItem)
            {
				configuration = new ModConfiguration();
				configuration.name = pItem.name;
				configuration.author = "";
				configuration.workshopID = pItem.id;
				configuration.isWorkshop = true;

				if(pItem.data != null)
					configuration.description = pItem.data.description;

				item = pItem;
            }
		}



		private List<WorkshopItem> _subscribedItems = new List<WorkshopItem>();
		private void RefreshSubscribedList()
        {
			_subscribedItems = Steam.GetAllWorkshopItems();
		}


		public void RebuildModList()
        {
			if (_lobby != null)
			{
				_mods = new List<Mod>();
				foreach (WorkshopItem i in _lobby.workshopItems)
				{
					_mods.Add(new UI_WorkshopItem(i));
				}
			}
			else
			{
				_mods = ModLoader.allMods.Where((a) => !(a is CoreMod) && (!serverModWindow || MonoMain.serverMods.Contains(a.configuration.workshopID))).ToList();

				if (serverModWindow == false)
				{
					// mod settings window button
					_mods.Insert(0, new UI_ModSettings());
					// null is special entry for "add more mods" button
					_mods.Add(null);
				}
			}

			RefreshSubscribedList();
		}

		public bool serverModWindow = false;
		public UIServerBrowser.LobbyData _lobby;
        public UIMenu _modSettingsMenu;
        public UIModManagement(UIMenu openOnClose, string title, float xpos, float ypos, float wide = -1.0f, float high = -1.0f, string conString = "", InputProfile conProfile = null, bool pServerModWindow = false, UIServerBrowser.LobbyData pLobby = null)
			: base(title, xpos, ypos, wide, high, conString, conProfile)
		{
			_lobby = pLobby;
			serverModWindow = pServerModWindow;
			_splitter.topSection.components[0].align = UIAlign.Left;

			_openOnClose = openOnClose;
			
			_moreArrow = new Sprite("moreArrow");
			_moreArrow.CenterOrigin();

			_steamIcon = new Sprite("steamIconSmall");
			_steamIcon.scale = new Vec2(1) / 2;

			_localIcon = new SpriteMap("iconSheet", 16, 16);
			_localIcon.scale = new Vec2(1) / 2;
			_localIcon.SetFrameWithoutReset(1);

            _modErrorIcon = new Sprite("modloadError");

            _newIcon = new SpriteMap("presents", 16, 16);
			_newIcon.scale = new Vec2(2);
			_newIcon.SetFrameWithoutReset(0);

            _settingsIcon = new SpriteMap("settingsWrench", 16, 16);
            _settingsIcon.scale = new Vec2(2);

            _noImage = new Sprite("notexture");
			_noImage.scale = new Vec2(2);

			_cursor = new SpriteMap("cursors", 16, 16);


			RebuildModList();

			_maxModsToShow = 8;

			_box = new UIBox(0, 0, -1, _maxModsToShow * boxHeight, true, false);
			Add(_box);


			_fancyFont = new FancyBitmapFont("smallFont");
			_fancyFont.maxWidth = (int)width - 60;
			_fancyFont.maxRows = 2;

			scrollBarOffset = 0;

			_editModMenu = new UIMenu("<mod name>", Layer.HUD.camera.width / 2, Layer.HUD.camera.height / 2, 240, -1, "@SELECT@SELECT");
			_editModMenu.Add(_disableOrEnableItem = new UIMenuItem("DISABLE", new UIMenuActionCallFunction(EnableDisableMod)));
			_deleteOrUnsubItem = new UIMenuItem("DELETE", new UIMenuActionCallFunction(DeleteMod));
			_uploadItem = new UIMenuItem("UPLOAD", new UIMenuActionCallFunction(UploadMod));
			_visitItem = new UIMenuItem("VISIT PAGE", new UIMenuActionCallFunction(VisitModPage));
			_editModMenu.Add(new UIText(" ", Color.White));
			_editModMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_editModMenu, this)));
			_editModMenu.Close();

			_yesNoMenu = new UIMenu("ARE YOU SURE?", Layer.HUD.camera.width / 2.0f, Layer.HUD.camera.height / 2.0f, 160, -1, "@SELECT@SELECT");
			_yesNoMenu.Add(_yesNoYes = new UIMenuItem("YES"));
			_yesNoMenu.Add(_yesNoNo = new UIMenuItem("NO"));
			_yesNoMenu.Close();

			_updateTextBox = new Textbox(0, 0, 0, 0, 1.0f);
			_updateTextBox.depth = 0.9f;
			_updateTextBox.maxLength = 5000;


            _modSettingsMenu = new UIMenu("@WRENCH@MOD SETTINGS@SCREWDRIVER@", Layer.HUD.camera.width / 2.0f, Layer.HUD.camera.height / 2.0f, 280, -1, "@WASD@ADJUST @CANCEL@EXIT");
            _modSettingsMenu.Add(new UIText("If CRASH DISABLE is ON,", Colors.DGBlue));
            _modSettingsMenu.Add(new UIText("a mod will automatically be", Colors.DGBlue));
            _modSettingsMenu.Add(new UIText(" disabled if it causes", Colors.DGBlue));
            _modSettingsMenu.Add(new UIText("the game to crash.", Colors.DGBlue));
            _modSettingsMenu.Add(new UIText(" ", Colors.DGBlue));
            _modSettingsMenu.Add(new UIMenuItemToggle("CRASH DISABLE", null, new FieldBinding(Options.Data, "disableModOnCrash", 0.0f, 1.0f)));
            _modSettingsMenu.Add(new UIMenuItemToggle("LOAD FAILURE DISABLE", null, new FieldBinding(Options.Data, "disableModOnLoadFailure", 0.0f, 1.0f)));
            _modSettingsMenu.Add(new UIMenuItemToggle("SHOW NETWORK WARNING", null, new FieldBinding(Options.Data, "showNetworkModWarning", 0.0f, 1.0f)));

            _modSettingsMenu.Add(new UIText(" ", Colors.DGBlue));
            _modSettingsMenu.Add(new UIMenuItem("BACK", new UIMenuActionOpenMenu(_modSettingsMenu, this), UIAlign.Center, default(Color), true));
            _modSettingsMenu.Close();
        }

		public override void Open()
		{
			if (_uploadDialog == null)
			{
				_uploadDialog = new SteamUploadDialog();
				Level.Add(_uploadDialog);
				Level.current.things.RefreshState();
			}

			_pressWait = 30;
            base.Open();
			DevConsole.SuppressDevConsole();
			_oldPos = Mouse.positionScreen;
		}

		bool _showingMenu = false;
		bool _draggingScrollbar;
		Vec2 _oldPos;

		Mod _selectedMod;

        bool modsChanged = false;

        public string showingError;
		public override void Update()
		{
			if (_uploadDialog != null && _uploadDialog.opened)
			{
				Editor.clickedMenu = false;
				Editor.inputMode = EditorInput.Mouse;
				Level.current.things.RefreshState();
				foreach (ContextMenu m in Level.current.things[typeof(ContextMenu)])
					m.Update();

				return;
            }

			if (_pressWait > 0)
				--_pressWait;

            if (showingError != null)
            {
                _controlString = "@CANCEL@BACK";
                if (Input.Pressed(Triggers.Quack))
                    showingError = null;

                base.Update();
                return;
            }

            if (_editModMenu.open)
			{
				if (UIMenu.globalUILock == false && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.Escape)))
				{
					_editModMenu.Close();
					Open();
					return;
				}
            }
            else if (_modSettingsMenu.open)
            {
                if (UIMenu.globalUILock == false && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.Escape)))
                {
                    _modSettingsMenu.Close();
                    Open();
                    return;
                }
            }
            else if (open)
			{
				if (_transferItem != null && !_needsUpdateNotes)
				{
					if (_transferring == false)
					{
						if (_transferItem.result == SteamResult.OK)
						{
							WorkshopItemData data = new WorkshopItemData();

							if (_selectedMod.configuration.workshopID == 0)
							{
								_selectedMod.configuration.SetWorkshopID(_transferItem.id);

								data.name = _selectedMod.configuration.displayName;
								data.description = _selectedMod.configuration.description;
								data.visibility = RemoteStoragePublishedFileVisibility.Private;
								data.tags = new List<string>();
								data.tags.Add("Mod");

                                if(_selectedMod.configuration.modType == ModConfiguration.Type.MapPack)
                                    data.tags.Add("Map Pack");
                                else if (_selectedMod.configuration.modType == ModConfiguration.Type.HatPack)
                                    data.tags.Add("Hat Pack");
                                else if (_selectedMod.configuration.modType == ModConfiguration.Type.Reskin)
                                    data.tags.Add("Texture Pack");
                            }
                            else
								data.changeNotes = _updateTextBox.text;


							string screenshotPath = _selectedMod.generateAndGetPathToScreenshot;
                            data.previewPath = screenshotPath;

							string folderPath = DuckFile.workshopDirectory + _transferItem.id + "/content";

							if (Directory.Exists(folderPath))
								Directory.Delete(folderPath, true);

							DuckFile.CreatePath(folderPath);

							DirectoryCopy(_selectedMod.configuration.directory, folderPath + "/" + _selectedMod.configuration.name, true);

							// delete "build" and ".vs" directory if it exists
							if (Directory.Exists(folderPath + _selectedMod.configuration.name + "/build"))
								Directory.Delete(folderPath + _selectedMod.configuration.name + "/build", true);

							if (Directory.Exists(folderPath + _selectedMod.configuration.name + "/.vs"))
								Directory.Delete(folderPath + _selectedMod.configuration.name + "/.vs", true);

							// delete compiled dll/hash if it exists 
                            if (File.Exists(folderPath + _selectedMod.configuration.name + "/" + _selectedMod.configuration.name + "_compiled.dll"))
                            {
                                string fName = folderPath + _selectedMod.configuration.name + "/" + _selectedMod.configuration.name + "_compiled.dll";
                                File.SetAttributes(fName, FileAttributes.Normal);
                                File.Delete(fName);
                            }

                            if (File.Exists(folderPath + _selectedMod.configuration.name + "/" + _selectedMod.configuration.name + "_compiled.hash"))
                            {
                                string fName = folderPath + _selectedMod.configuration.name + "/" + _selectedMod.configuration.name + "_compiled.hash";
                                File.SetAttributes(fName, FileAttributes.Normal);
                                File.Delete(fName);
                            }

							data.contentFolder = folderPath;

							_transferItem.ApplyWorkshopData(data);

							if (_transferItem.needsLegal)
								Steam.ShowWorkshopLegalAgreement(_transferItem.id.ToString());

							_transferring = true;
							_transferItem.ResetProcessing();
                        }
					}
					else
					{
						if (_transferItem.finishedProcessing)
						{
							Steam.OverlayOpenURL("http://steamcommunity.com/sharedfiles/filedetails/?id=" + _transferItem.id);

							// delete workshop temp folder
							string folderPath = DuckFile.workshopDirectory + _transferItem.id + "/";
							Directory.Delete(folderPath, true);

							_transferItem.ResetProcessing();
							_transferItem = null;
							_transferring = false;
                        }
					}

					base.Update();
					return;
				}

				// set local selected
				if (_gamepadMode)
				{
					if (_hoverIndex < 0)
						_hoverIndex = 0;
				}
				else
				{
					_hoverIndex = -1;

					for (var i = 0; i < _maxModsToShow; ++i)
					{
						if (_scrollItemOffset + i >= _mods.Count)
							break;

						var boxLeft = _box.x - _box.halfWidth;
						var boxTop = (_box.y - _box.halfHeight) + (boxHeight * i);
						var rect = new Rectangle((int)boxLeft, (int)boxTop, (int)_box.width - boxSideMargin, boxHeight);

						if (rect.Contains(Mouse.position))
						{
							_hoverIndex = _scrollItemOffset + i;
							break;
						}
					}
				}

				if (_transferItem != null)
				{
					if (_updateTextBox != null)
					{
						Editor.hoverTextBox = false;

						_updateTextBox.position = new Vec2(_box.x - _box.halfWidth + 16, (_box.y - _box.halfHeight) + 48);
						_updateTextBox.size = new Vec2(_box.width - 32, _box.height - 80);
						_updateTextBox._maxLines = (int)(_updateTextBox.size.y / _fancyFont.characterHeight);
						_updateTextBox.Update();

						var sw = Graphics.GetStringWidth(_updateButtonText, false, 2);
						var sh = Graphics.GetStringHeight(_updateButtonText) * 2;

						_updateButton = new Rectangle(_box.x - (sw / 2), (_box.y + _box.halfHeight) - 24, sw, sh);

						if (_updateButton.Contains(Mouse.position) && Mouse.left == InputState.Pressed)
						{
							_needsUpdateNotes = false;
							_updateTextBox.LoseFocus();
						}
						else if (Keyboard.Pressed(Keys.Escape))
						{
							_needsUpdateNotes = false;
							_transferItem = null;
							_updateTextBox.LoseFocus();

							UIMenuActionOpenMenu ac = new UIMenuActionOpenMenu(this, _editModMenu);
							ac.Activate();
							return;
						}
					}
				}
				else if (_hoverIndex != -1)
				{
					_selectedMod = _mods[_hoverIndex];

					bool serverMod = IsServerMod(_selectedMod);
					if (serverMod)
					{
						if (_subscribedItems.FirstOrDefault(x => x.id == _selectedMod.configuration.workshopID) != null)
							_controlString = "@WASD@@SELECT@ADJUST @MENU1@UNSUBSCRIBE @CANCEL@BACK";
						else
							_controlString = "@WASD@@SELECT@ADJUST @MENU1@SUBSCRIBE @CANCEL@BACK";
					}
					else
					{
						if (_selectedMod is UI_ModSettings)
						{
							_controlString = "@WASD@@SELECT@SETTINGS @CANCEL@BACK";
						}
						else
						{
							if (_selectedMod != null && _selectedMod.configuration.error != null)
							{
								if (_selectedMod.configuration.forceHarmonyLegacyLoad)
									_controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @MENU2@DISABLE FORCED LOAD @START@SHOW ERROR";
								else
									_controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @MENU2@FORCE LEGACY LOAD @START@SHOW ERROR";
							}
							else
								_controlString = "@WASD@@SELECT@ADJUST @MENU1@TOGGLE @CANCEL@BACK";
						}
					}


                    if (Input.Pressed(Triggers.Menu1))
                    {
                        if (_selectedMod != null && _selectedMod.configuration != null)
                        {
							if (serverMod)
							{
								if (_subscribedItems.FirstOrDefault(x => x.id == _selectedMod.configuration.workshopID) != null)
								{
									Steam.WorkshopUnsubscribe(_selectedMod.configuration.workshopID);
									_subscribedItems.Remove(WorkshopItem.GetItem(_selectedMod.configuration.workshopID));
								}
								else
								{
									Steam.WorkshopSubscribe(_selectedMod.configuration.workshopID);
									_subscribedItems.Add(WorkshopItem.GetItem(_selectedMod.configuration.workshopID));
								}
							}
							else
							{
								if (_selectedMod.configuration.disabled)
									_selectedMod.configuration.Enable();
								else
									_selectedMod.configuration.Disable();
								_selectedMod.configuration.error = null;
							}

                           modsChanged = true;

                            SFX.Play("rockHitGround", 0.8f);
                        }
                    }
                    else if (_selectedMod != null && _selectedMod.configuration != null && _selectedMod.configuration.error != null && Input.Pressed(Triggers.Menu2) && !serverMod)
                    {
                        if (_selectedMod.configuration != null)
                        {
                            _selectedMod.configuration.forceHarmonyLegacyLoad = !_selectedMod.configuration.forceHarmonyLegacyLoad;
                            ModLoader.DisabledModsChanged();
                            modsChanged = true;
                            SFX.Play("rockHitGround", 0.8f);
                        }
                    }
                    else if (Input.Pressed(Triggers.Start) && !serverMod && _selectedMod != null && _selectedMod.configuration != null && _selectedMod.configuration.error != null)
                    {
						string errorPath = DuckFile.saveDirectory + "error_info.txt";
						File.WriteAllText(errorPath, _selectedMod.configuration.error);
						Process.Start(errorPath);
						//showingError = _selectedMod.configuration.error;
                        SFX.Play("rockHitGround", 0.8f);
                        return;
                    }
                    else if ((Input.Pressed(Triggers.Select) && !serverMod && _pressWait == 0 && _gamepadMode) || (!_gamepadMode && !serverMod && Mouse.left == InputState.Pressed))
					{
						if (_selectedMod != null)
						{
                            if (_selectedMod is UI_ModSettings)
                            {
                                SFX.Play("rockHitGround", 0.8f);

                                _modSettingsMenu.dirty = true;
                                UIMenuActionOpenMenu ac = new UIMenuActionOpenMenu(this, _modSettingsMenu);
                                ac.Activate();
                                return;
                            }
                            else
                            {

                                if (!_selectedMod.configuration.loaded)
                                    _editModMenu.title = "|YELLOW|" + _selectedMod.configuration.name;
                                else
                                    _editModMenu.title = "|YELLOW|" + _selectedMod.configuration.displayName;

                                _editModMenu.Remove(_deleteOrUnsubItem);
                                _editModMenu.Remove(_uploadItem);
                                _editModMenu.Remove(_visitItem);

                                if (!_selectedMod.configuration.isWorkshop && _selectedMod.configuration.loaded)
                                {
                                    if (_selectedMod.configuration.workshopID != 0)
                                        _uploadItem.text = "UPDATE";
                                    else
                                        _uploadItem.text = "UPLOAD";

                                    _editModMenu.Insert(_uploadItem, 1);
                                }

                                if ((!_selectedMod.configuration.isWorkshop && !_selectedMod.configuration.loaded))
                                {
                                    _deleteOrUnsubItem.text = "DELETE";
                                    _editModMenu.Insert(_deleteOrUnsubItem, 1);
                                }
                                else if (_selectedMod.configuration.isWorkshop)
                                {
                                    _deleteOrUnsubItem.text = "UNSUBSCRIBE";
                                    _editModMenu.Insert(_deleteOrUnsubItem, 1);
                                }

                                if (_selectedMod.configuration.isWorkshop)
                                    _editModMenu.Insert(_visitItem, 1);

                                _disableOrEnableItem.text = (_selectedMod.configuration.disabled) ? "ENABLE" : "DISABLE";
                                _editModMenu.dirty = true;

                                SFX.Play("rockHitGround", 0.8f);

                                UIMenuActionOpenMenu ac = new UIMenuActionOpenMenu(this, _editModMenu);
                                ac.Activate();
                                return;
                            }
						}
						else
							Steam.OverlayOpenURL("http://steamcommunity.com/workshop/browse/?appid=312530&searchtext=&childpublishedfileid=0&browsesort=trend&section=readytouseitems&requiredtags%5B%5D=Mod");
					}
				}
				else
					_selectedMod = null;

				if (_gamepadMode)
				{
					_draggingScrollbar = false;

					if (Input.Pressed(Triggers.MenuDown))
						_hoverIndex++;
					else if (Input.Pressed(Triggers.MenuUp))
						_hoverIndex--;
					if (Input.Pressed(Triggers.Strafe))
						_hoverIndex -= 8;
					else if (Input.Pressed(Triggers.Ragdoll))
						_hoverIndex += 8;

					if (_hoverIndex < 0)
						_hoverIndex = 0;

					if ((_oldPos - Mouse.positionScreen).lengthSq > 200)
						_gamepadMode = false;
				}
				else
				{
					if (_draggingScrollbar == false)
					{
						if (Mouse.left == InputState.Pressed && ScrollBarBox().Contains(Mouse.position))
						{
							_draggingScrollbar = true;
							_oldPos = Mouse.position;
						}

						if (Mouse.scroll > 0)
						{
							_scrollItemOffset += 5;
							_hoverIndex += 5;
						}
						else if (Mouse.scroll < 0)
						{
							_scrollItemOffset -= 5;
							_hoverIndex -= 5;

							if (_hoverIndex < 0)
								_hoverIndex = 0;
						}
					}
					else
					{
						if (Mouse.left != InputState.Down)
							_draggingScrollbar = false;
						else
						{
							var delta = Mouse.position - _oldPos;
							_oldPos = Mouse.position;

							scrollBarOffset += (int)delta.y;

							if (scrollBarOffset > scrollBarScrollableHeight)
								scrollBarOffset = scrollBarScrollableHeight;
							else if (scrollBarOffset < 0)
								scrollBarOffset = 0;

							var heightScrolled = (float)scrollBarOffset / scrollBarScrollableHeight;
							_scrollItemOffset = (int)((_mods.Count - _maxModsToShow) * heightScrolled);
						}
                    }

					if (Input.Pressed(Triggers.Any))
					{
						_gamepadMode = true;
						_oldPos = Mouse.positionScreen;
                    }
				}

				if (_scrollItemOffset < 0)
					_scrollItemOffset = 0;
				else if (_scrollItemOffset > Math.Max(0, _mods.Count - _maxModsToShow))
					_scrollItemOffset = Math.Max(0, _mods.Count - _maxModsToShow);

				if (_hoverIndex >= _mods.Count)
					_hoverIndex = _mods.Count - 1;
				else if (_hoverIndex >= _scrollItemOffset + _maxModsToShow)
					_scrollItemOffset += _hoverIndex - (_scrollItemOffset + _maxModsToShow) + 1;
				else if (_hoverIndex >= 0 && _hoverIndex < _scrollItemOffset)
					_scrollItemOffset -= (_scrollItemOffset - _hoverIndex);

                if (!_draggingScrollbar)
                {
                    if (_scrollItemOffset != 0)
                        scrollBarOffset = (int)Lerp.FloatSmooth(0, scrollBarScrollableHeight, _scrollItemOffset / (float)(_mods.Count - _maxModsToShow));
                    else
                        scrollBarOffset = 0;
                }

				if (Editor.hoverTextBox == false && UIMenu.globalUILock == false && (Input.Pressed(Triggers.Cancel) || Keyboard.Pressed(Keys.Escape)))
				{
                    if (modsChanged && !serverModWindow)
                    {
                        Close();
                        Main.pauseMenu = DuckNetwork.OpenModsRestartWindow(_openOnClose);
                    }
                    else
                    {
                        UIMenuActionOpenMenu ac = new UIMenuActionOpenMenu(this, _openOnClose);
                        ac.Activate();
                    }

                    modsChanged = false;
                    return;
				}
			}

			if (_showingMenu == true)
			{
				HUD.CloseAllCorners();
				_showingMenu = false;
			}

			base.Update();
		}
		
		bool fixView = true;
		const int boxHeight = 36;
		const int scrollWidth = 12;
		const int boxSideMargin = scrollWidth + 2;

		const int scrollBarHeight = 32;

		int scrollBarTop, scrollBarBottom;
		int scrollBarScrollableHeight;
		int scrollBarOffset;

		int _scrollItemOffset = 0;

		bool _gamepadMode = true;
		bool _needsUpdateNotes = false;

		Rectangle ScrollBarBox()
		{
			return new Rectangle(_box.x + _box.halfWidth - scrollWidth + 1, _box.y - _box.halfHeight + 1 + scrollBarOffset, scrollWidth - 2, scrollBarHeight);
        }

		public bool IsServerMod(Mod mod)
        {
			if (mod != null && mod.configuration != null && mod.configuration.isWorkshop && _subscribedItems.FirstOrDefault(x => x.id == mod.configuration.workshopID) == null)
				return true;

			return false;
		}

		public override void Draw()
		{

			if (open)
            {
				if (Mouse.available && !_gamepadMode)
				{
					_cursor.depth = 1.0f;
					_cursor.scale = new Vec2(1.0f, 1.0f);
					_cursor.position = Mouse.position;

					_cursor.frame = 0;

					if (Editor.hoverTextBox)
					{
						_cursor.frame = 7;
						_cursor.position.y -= 4.0f;
						_cursor.scale = new Vec2(0.5f, 1.0f);
					}

					_cursor.Draw();
				}

				if (_uploadDialog != null && _uploadDialog.opened)
				{
					Editor.hoverTextBox = false;
					_gamepadMode = false;
					foreach (ContextMenu m in Level.current.things[typeof(ContextMenu)])
						m.Draw();

					return;
				}


				if (showingError != null)
                {
                    var boxLeft = _box.x - _box.halfWidth;
                    var boxTop = (_box.y - _box.halfHeight);
                    _fancyFont.scale = new Vec2(1.0f);

                    int wide = _fancyFont.maxWidth;
                    _fancyFont.maxRows = 40;
                    _fancyFont.maxWidth = (int)width - 10; 
                    _fancyFont.Draw(showingError, new Vec2(boxLeft + 4, boxTop + 4), Color.White, 0.5f);
                    _fancyFont.maxRows = 2;
                    _fancyFont.maxWidth = wide;

                    base.Draw();
                    return;
                }

				scrollBarTop = (int)(_box.y - _box.halfHeight + 1 + (scrollBarHeight / 2));
				scrollBarBottom = (int)(_box.y + _box.halfHeight - 1 - (scrollBarHeight / 2));
				scrollBarScrollableHeight = scrollBarBottom - scrollBarTop;

				if (fixView)
				{
					Layer.HUD.camera.width *= 2;
					Layer.HUD.camera.height *= 2;
					fixView = false;
                }

				//float yZone = (14 * 1) - 12;

				//_moreArrow.depth = depth + 2;
				//_moreArrow.flipV = true;
				//Graphics.Draw(_moreArrow, x, y - (yZone / 2) - 2);

				Graphics.DrawRect(new Vec2(_box.x - _box.halfWidth, _box.y - _box.halfHeight), new Vec2(_box.x + _box.halfWidth - scrollWidth - 2, _box.y + _box.halfHeight), Color.Black, 0.4f);

				Graphics.DrawRect(new Vec2(_box.x + _box.halfWidth - scrollWidth, _box.y - _box.halfHeight), new Vec2(_box.x + _box.halfWidth, _box.y + _box.halfHeight), Color.Black, 0.4f);

				var sb = ScrollBarBox();
                Graphics.DrawRect(sb, (_draggingScrollbar || sb.Contains(Mouse.position)) ? Color.LightGray : Color.Gray, 0.5f);

				for (var i = 0; i < _maxModsToShow; ++i)
				{
					var modIndex = _scrollItemOffset + i;

					if (modIndex >= _mods.Count)
						break;

					var boxLeft = _box.x - _box.halfWidth;
					var boxTop = (_box.y - _box.halfHeight) + (boxHeight * i);

					if (_transferItem == null && _hoverIndex == modIndex)
						Graphics.DrawRect(new Vec2(boxLeft, boxTop), new Vec2(boxLeft + _box.width - boxSideMargin, boxTop + boxHeight), Color.White * 0.6f, 0.45f);
					else if ((modIndex & 1) != 0)
						Graphics.DrawRect(new Vec2(boxLeft, boxTop), new Vec2(boxLeft + _box.width - boxSideMargin, boxTop + boxHeight), Color.White * 0.1f, 0.45f);

					var mod = _mods[modIndex];

					if (mod != null)
					{
                        if (mod is UI_ModSettings)
                        {
                            Graphics.Draw(_settingsIcon, boxLeft + 2, boxTop + 1, 0.5f);

                            _fancyFont.scale = new Vec2(1.5f);
                            _fancyFont.Draw("Mod Settings", new Vec2(boxLeft + 36, boxTop + 11), Color.White, 0.5f);
                            _fancyFont.scale = new Vec2(1);
                        }
                        else
                        {
                            var preview = mod.previewTexture;

							if (mod is UI_WorkshopItem)
								preview = UIServerBrowser.GetWorkshopPreview((mod as UI_WorkshopItem).item);

                            if (preview != null && _noImage.texture != preview)
                            {
                                _noImage.texture = preview;
                                _noImage.scale = new Vec2(32.0f / preview.width);
                            }

                            Graphics.DrawRect(new Vec2(boxLeft + 2, boxTop + 2), new Vec2(boxLeft + boxHeight - 2, boxTop + boxHeight - 2), Color.Gray, 0.44f, false, 2);
                            Graphics.Draw(_noImage, boxLeft + 2, boxTop + 2, 0.5f);

                            string titleString = "#" + modIndex + ": ";
                            if (mod.clientMod)
                                titleString = titleString.Insert(0,"|DGYELLOW|");
                                

                            if (mod.configuration.error != null)
                            {
                                _modErrorIcon.scale = new Vec2(2.0f);
                                Graphics.Draw(_modErrorIcon, boxLeft + 2, boxTop + 2, 0.55f);
                                titleString += "|DGRED|";
                            }

                            if(mod.configuration.error != null || mod.configuration.disabled)
                                Graphics.DrawRect(new Vec2(boxLeft, boxTop), new Vec2(boxLeft + _box.width - boxSideMargin, boxTop + boxHeight), Color.Black * 0.4f, 0.85f);


                            bool reskin = mod.configuration.modType == ModConfiguration.Type.Reskin || mod.configuration.isExistingReskinMod;


							if (!mod.configuration.loaded)
								titleString += mod.configuration.name;
							else
							{
								if(reskin)
									titleString += mod.configuration.displayName + "|WHITE| by |PURPLE|" + mod.configuration.author;
								else
									titleString += mod.configuration.displayName + "|WHITE| v" + mod.configuration.version.ToString() + " by |PURPLE|" + mod.configuration.author;
							}

                            if (reskin)
                                titleString += "|DGPURPLE| (Reskin Pack)";
                            else if (mod.configuration.modType == ModConfiguration.Type.MapPack)
                                titleString += "|DGPURPLE| (Map Pack)";
                            else if (mod.configuration.modType == ModConfiguration.Type.HatPack)
                                titleString += "|DGPURPLE| (Hat Pack)";

							if (serverModWindow)
							{
								if (!IsServerMod(mod))
									titleString += "|DGGREEN| (Subscribed)";
							}
							else
							{
								if (IsServerMod(mod))
									titleString += "|DGORANGE| (Server Mod)";
								else
									titleString += mod.configuration.disabled ? "|DGRED| (Disabled)" : "|DGGREEN| (Enabled)";
							}

							_fancyFont.Draw(titleString, new Vec2(boxLeft + 36 + 10, boxTop + 2), Color.Yellow, 0.5f);

                            Graphics.Draw((!mod.configuration.isWorkshop) ? _localIcon : _steamIcon, boxLeft + 36, boxTop + 2.5f, 0.5f);

                            if (mod.configuration.error != null && (mod.configuration.disabled || mod is ErrorMod))
                            {
                                string er = mod.configuration.error;
                                if (er.Length > 150)
                                    er = er.Substring(0, 150);

                                _fancyFont.Draw((mod.configuration.error.StartsWith("!") ? ("|DGYELLOW|" + er.Substring(1, er.Length - 1)) : ("|DGRED|Failed with error: " + er)), new Vec2(boxLeft + 36, boxTop + 6 + _fancyFont.characterHeight), Color.White, 0.5f);
                            }
                            else
                            {

								if (mod is UI_WorkshopItem)
									_fancyFont.Draw(mod.configuration.description, new Vec2(boxLeft + 36, boxTop + 6 + _fancyFont.characterHeight), Color.White, 0.5f);
								else
								{
									if (!mod.configuration.loaded)
									{
										if (mod.configuration.disabled)
											_fancyFont.Draw("Mod is disabled.", new Vec2(boxLeft + 36, boxTop + 6 + _fancyFont.characterHeight), Color.LightGray, 0.5f);
										else
											_fancyFont.Draw("|DGGREEN|Mod will be enabled on next restart.", new Vec2(boxLeft + 36, boxTop + 6 + _fancyFont.characterHeight), Color.Orange, 0.5f);
									}
									else
									{
										if (mod.configuration.disabled)
											_fancyFont.Draw("|DGRED|Mod will be disabled on next restart.", new Vec2(boxLeft + 36, boxTop + 6 + _fancyFont.characterHeight), Color.Orange, 0.5f);
										else
											_fancyFont.Draw(mod.configuration.description, new Vec2(boxLeft + 36, boxTop + 6 + _fancyFont.characterHeight), Color.White, 0.5f);
									}
								}
                            }
                        }
					}
					else
					{
						Graphics.Draw(_newIcon, boxLeft + 2, boxTop + 1, 0.5f);

						_fancyFont.scale = new Vec2(1.5f);
						_fancyFont.Draw("Get " + (_mods.Count == 1 ? "some" : "more") + " mods!", new Vec2(boxLeft + 36, boxTop + 11), Color.White, 0.5f);
						_fancyFont.scale = new Vec2(1);
					}
				}

				if (_awaitingChanges)
					Graphics.DrawString("Restart required for some changes to take effect!", new Vec2(x - halfWidth + 128, y - halfHeight + 8), Color.Red, 0.6f, null);

				if (_transferItem != null)
				{
					Graphics.DrawRect(new Rectangle(_box.x - _box.halfWidth, _box.y - _box.halfHeight, _box.width, _box.height), Color.Black * 0.9f, 0.7f);

					string centerTopText = "Creating item...";

					if (_transferring)
					{
						var pct = _transferItem.GetUploadProgress();

						switch (pct.status)
						{
							default:
								centerTopText = "Waiting";
								break;
							case ItemUpdateStatus.CommittingChanges:
								centerTopText = "Committing changes";
								break;
							case ItemUpdateStatus.PreparingConfig:
								centerTopText = "Preparing config";
								break;
							case ItemUpdateStatus.PreparingContent:
								centerTopText = "Preparing content";
								break;
							case ItemUpdateStatus.UploadingContent:
								centerTopText = "Uploading content";
								break;
							case ItemUpdateStatus.UploadingPreviewFile:
								centerTopText = "Uploading preview";
								break;
						}

						if (pct.bytesTotal != 0)
						{
							var percent = ((float)pct.bytesDownloaded / (float)pct.bytesTotal);
							centerTopText += " (" + (int)(percent * 100) + "%)";

							Graphics.DrawRect(new Rectangle(_box.x - _box.halfWidth + 8, _box.y - 8, _box.width - 16, 16), Color.LightGray, 0.8f);

							Graphics.DrawRect(new Rectangle(_box.x - _box.halfWidth + 8, _box.y - 8, Lerp.FloatSmooth(0, _box.width - 16, percent), 16), Color.Green, 0.8f);
						}

						centerTopText += "...";
					}
					else if (_needsUpdateNotes)
					{
						Graphics.DrawRect(new Rectangle(_updateTextBox.position.x - 1, _updateTextBox.position.y - 1, _updateTextBox.size.x + 2, _updateTextBox.size.y + 2), Color.Gray, 0.85f, false, 1);
						Graphics.DrawRect(new Rectangle(_updateTextBox.position.x, _updateTextBox.position.y, _updateTextBox.size.x, _updateTextBox.size.y), Color.Black, 0.85f);
						_updateTextBox.Draw();
						centerTopText = "Enter change notes:";

						Graphics.DrawString(_updateButtonText, new Vec2(_updateButton.x, _updateButton.y), _updateButton.Contains(Mouse.position) ? Color.Yellow : Color.White, 0.9f, null, 2);
					}

					var width = Graphics.GetStringWidth(centerTopText, false, 2);
					Graphics.DrawString(centerTopText, new Vec2(_box.x - (width / 2), (_box.y - _box.halfHeight + 24)), Color.White, 0.8f, null, 2);
				}
			}

			base.Draw();
		}
	}
}
