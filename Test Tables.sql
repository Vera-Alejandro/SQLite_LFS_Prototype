CREATE TABLE IF NOT EXISTS ExtensionInfo
(
	Id INTEGER PRIMARY KEY NOT NULL,
	Extension TEXT
);

INSERT INTO ExtensionInfo (Extension) VALUES ('txt'), ('exe'), ('pdf'), ('jpg'), ('cs'), ('c'), ('cpp');

CREATE TABLE IF NOT EXISTS FileData
(
	Id INTEGER PRIMARY KEY NOT NULL,
	Name TEXT,
	Data TEXT,
	ExtensionId INTEGER,
	FOREIGN KEY (ExtensionId) REFERENCES ExtensionInfo(Id)
);

INSERT INTO FileData (Name, Data, ExtensionId) VALUES ('Battleship', 'attack(); Play(); funcion();', 5), ('GreatGraph', 'tree stuff if(not tree == do nothing', 7), ('GameFinal', 'this is my csc250 Final', 6), ('Security Report', 'you cant do anything about it youre going to get hacked lol', 1), ('memegato', '*here lies funny cat meme*', 4), ('Scholarship Reward', 'you just got a full ride for being really really cool', 3);