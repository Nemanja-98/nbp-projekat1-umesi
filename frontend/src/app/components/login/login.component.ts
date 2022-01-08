import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit } from '@angular/core';

// import { AccountService } from '../../../services/account/account.service';
// import { AlertService  } from '../../../services/alert/alert.service';
import { first } from 'rxjs/operators';

@Component({ templateUrl: './login.component.html',  styleUrls: ['./login.component.css'] })
export class LoginComponent implements OnInit {
    form: FormGroup;
    loading = false;
    submitted = false;
    returnUrl: string;
    error = '';

    constructor(
        
        // private accountService: AccountService,
        // private alertService: AlertService,
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
    ) { 
        // if (this.accountService.userValue) { 
        //     this.router.navigate(['/']);
        // }
    }

    ngOnInit() {
        this.form = this.formBuilder.group({
            username: ['', Validators.required],
            password: ['', Validators.required]
        });

        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    get f() { return this.form.controls; }

    async onSubmit() {
        this.submitted = true;

        // this.alertService.clear();

        if (this.form.invalid) {
            return;
        }

         this.loading = true;
        console.log("user:",this.f.username.value,this.f.password.value);
        let response =  '';
        //  const response =  await this.accountService.login(this.f.username.value, this.f.password.value);
         console.log("return value",response);
         
        // setTimeout(() => {
        //     console.log("awaited response in login:",response);
        //     this.loading = false;
            
        // }, 2000);
        // response.pipe(first())
        // .subscribe(
        //     data => {
        //         this.router.navigate([this.returnUrl]);
        //     },
        //     error => {
        //         this.alertService.error(error);
        //         this.loading = false;
        //     }); 
     }
}



            // .pipe(first())
            // .subscribe(
            //     data => {
            //         this.router.navigate([this.returnUrl]);
            //     },
            //     error => {
            //         this.alertService.error(error);
            //         this.loading = false;
            //     });