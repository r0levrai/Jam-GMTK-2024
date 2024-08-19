import { Module } from '@nestjs/common';
import { DrawingController } from './drawing.controller';
import { DrawingService } from './drawing.service';
import { DrawingRepository } from './drawing.repository';
import { TypeOrmExModule } from 'src/database/typeorm-ex.module';

@Module({
  imports: [TypeOrmExModule.forCustomRepository([DrawingRepository])],
  controllers: [DrawingController],
  providers: [DrawingService],
})
export class DrawingModule {}