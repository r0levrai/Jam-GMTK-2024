import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { ConfigModule } from '@nestjs/config';
import { TypeOrmModule } from '@nestjs/typeorm';
import { typeOrmConfig } from './config/typeorm.config';
import { DrawingModule } from './drawing/drawing.module';
import { ReactionModule } from './reaction/reaction.module';

@Module({
  imports: [
    TypeOrmModule.forRoot(typeOrmConfig),
    DrawingModule,
    ReactionModule
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
