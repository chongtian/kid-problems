export class MockMessageService {

  messages: string[] = [];

  constructor() { }

  add(message: string) {
    this.messages.push(message);
  }

  clear() {
    this.messages = [];
  }

  replace(message: string) {
    this.messages = [];
    this.messages.push(message);
  }

  openSnackBar(message: string) {
      console.log(`Open Mat Snack Bar and show ${message}`);
  }

}
