import { TestBed } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MessageService } from '@app/_services';


describe('Test MessageService', () => {
    // this is the service to be tested
    let messageService: MessageService;

    // prepare spy objects for all dependencies
    let matSnackBar: jasmine.SpyObj<MatSnackBar>;


    beforeEach(() => {
        // create spy on an object representing the MatSnackBar
        const spy = jasmine.createSpyObj('MatSnackBar', ['open']);

        TestBed.configureTestingModule({ providers: [MessageService, { provide: MatSnackBar, useValue: spy }] });

        // Inject both the service-to-test and its (spy) dependency
        // TestBed.get() was deprecated as of Angular version 9. Use TestBed.inject() after version 9
        messageService = TestBed.inject(MessageService);
        matSnackBar = TestBed.inject(MatSnackBar) as jasmine.SpyObj<MatSnackBar>;
    });

    it('add() adds message to MessageService', () => {
        messageService.add('Test');
        expect(messageService.messages[0])
            .toBe('Test');
    });

    it('clear() deletes all messages', () => {
        messageService.messages = [];
        messageService.messages.push('Test');
        messageService.clear();
        expect(messageService.messages.length).toBe(0);
    });

    it('replace() deletes all messages and add new message', () => {
        messageService.messages = [];
        messageService.messages.push('Test');
        messageService.replace('Test Again');
        expect(messageService.messages[0])
            .toBe('Test Again');
    });

    // I do not test openSnackBar() method

});
