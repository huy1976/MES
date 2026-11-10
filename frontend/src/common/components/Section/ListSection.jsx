import clsx from 'clsx';
import styles from './Section.module.scss';

const ListSection = ({ className = "", children, style}) => {
    return (
      <div className={clsx(styles.listSection, className)} style = {style}>
        {children}
      </div>
    );
  };
  
  export default ListSection;
  