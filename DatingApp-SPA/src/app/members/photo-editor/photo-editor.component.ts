import { Photo } from './../../_models/photo';
import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { User } from 'src/app/_models/user';
import { AuthService } from './../../_services/auth.service';
import { environment } from './../../../environments/environment';
import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
@Input() photos: Photo[];
@Output() getMemberChangePhoto = new EventEmitter<string>();
uploader: FileUploader  ;
hasBaseDropZoneOver = false;
baseUrl = environment.apiUrl;
currentMain: Photo;

  constructor(private authService: AuthService, private userService: UserService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

   fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader(){
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };
  }
  setMainPhoto(photo: Photo){
    this.userService.setMainPhoto(photo.id, this.authService.decodedToken.nameid).subscribe(() =>
    {console.log('main photo set successfully');
    // change selected photo to main without refreshing page
     this.currentMain = this.photos.filter(p => p.isMain === true)[0];
     this.currentMain.isMain = false;
     photo.isMain = true;
     // member-edit change photo
     this.getMemberChangePhoto.emit(photo.url); },
    error => {
      this.alertify.error(error);
    });
  }

  deletePhoto(id: number){
    this.userService.deletePhotos(id, this.authService.decodedToken.nameid)
    .subscribe(() => {
      this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
      this.alertify.success('photo deleted successfully');
    }, error => {
      this.alertify.error('Failed to delete the photo');
    });
  }
}

