IUI ui = new ConsoleUI();
IHandler handler = new GarageHandler();
GarageManager manager = new GarageManager(ui, handler);

manager.Run();