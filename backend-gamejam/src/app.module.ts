import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { ConfigModule } from '@nestjs/config';
import { TypeOrmModule } from '@nestjs/typeorm';
import { typeOrmConfig } from './config/typeorm.config';
import { DrawingModule } from './drawing/drawing.module';

@Module({
  imports: [
    TypeOrmModule.forRoot(typeOrmConfig),
    DrawingModule,
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
